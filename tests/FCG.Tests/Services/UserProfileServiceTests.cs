using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Tests.Services;

public class UserProfileServiceTests
{
    [Fact]
    public async Task ChangePassword_sucesso_rotaciona_credencial()
    {
        await using var db = await TestDatabase.CreateAsync();
        var hash = BCrypt.Net.BCrypt.HashPassword("Velha@123");
        var usuario = new Usuario("T", "t@test.com", hash);
        db.Db.Usuarios.Add(usuario);
        await db.Db.SaveChangesAsync();
        var userId = usuario.Id;
        var versaoAntes = usuario.CredencialVersao;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        await svc.ChangePasswordAsync(userId, new ChangePasswordRequest(
            "Velha@123",
            "Nova@456x",
            "Nova@456x"));

        var reloaded = await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId);
        reloaded.CredencialVersao.Should().BeGreaterThan(versaoAntes);
        BCrypt.Net.BCrypt.Verify("Nova@456x", reloaded.SenhaHash).Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_confirmacao_diferente_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var hash = BCrypt.Net.BCrypt.HashPassword("Velha@123");
        db.Db.Usuarios.Add(new Usuario("T", "t@test.com", hash));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        var act = async () => await svc.ChangePasswordAsync(userId, new ChangePasswordRequest(
            "Velha@123",
            "Nova@456x",
            "Outra@456x"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*confirmacao nao conferem*");
    }

    [Fact]
    public async Task ChangePassword_senha_atual_errada_lanca_UnauthorizedAccessException()
    {
        await using var db = await TestDatabase.CreateAsync();
        var hash = BCrypt.Net.BCrypt.HashPassword("Velha@123");
        db.Db.Usuarios.Add(new Usuario("T", "t@test.com", hash));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        var act = async () => await svc.ChangePasswordAsync(userId, new ChangePasswordRequest(
            "Errada@999",
            "Nova@456x",
            "Nova@456x"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task UpdateMyProfile_altera_nome_e_email_e_rotaciona_se_email_mudar()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("Antigo", "a@old.com", "h"));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;
        var v0 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        var res = await svc.UpdateMyProfileAsync(userId, new UpdateMyProfileRequest("Novo", "novo@email.com"));

        res.Name.Should().Be("Novo");
        res.Email.Should().Be("novo@email.com");

        var v1 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;
        v1.Should().BeGreaterThan(v0);
    }

    [Fact]
    public async Task UpdateMyProfile_mesmo_email_nao_rotaciona_credencial()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("N", "mesmo@email.com", "h"));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;
        var v0 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        await svc.UpdateMyProfileAsync(userId, new UpdateMyProfileRequest("Nome2", "mesmo@email.com"));

        var v1 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;
        v1.Should().Be(v0);
    }

    [Fact]
    public async Task UpdateMyProfile_email_ja_usado_por_outro_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("A", "a@x.com", "h"));
        db.Db.Usuarios.Add(new Usuario("B", "b@x.com", "h"));
        await db.Db.SaveChangesAsync();
        var idB = (await db.Db.Usuarios.FirstAsync(u => u.Email == "b@x.com")).Id;

        var repo = new UsuarioRepository(db.Db);
        var svc = new UserProfileService(repo, db.Db);
        var act = async () => await svc.UpdateMyProfileAsync(idB, new UpdateMyProfileRequest("B", "a@x.com"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*E-mail ja cadastrado*");
    }
}
