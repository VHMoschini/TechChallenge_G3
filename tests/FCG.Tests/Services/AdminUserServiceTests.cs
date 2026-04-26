using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Tests.Services;

public class AdminUserServiceTests
{
    [Fact]
    public async Task UpdateUserAsync_altera_dados_e_rotaciona_se_email_mudar()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("N", "old@x.com", "h"));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;
        var v0 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;

        var repo = new UsuarioRepository(db.Db);
        var admin = new AdminUserService(repo, db.Db);
        var res = await admin.UpdateUserAsync(userId, new UpdateUserAdminRequest("Nome2", "new@x.com"));

        res.Email.Should().Be("new@x.com");
        var v1 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;
        v1.Should().BeGreaterThan(v0);
    }

    [Fact]
    public async Task UpdateRoleAsync_rotaciona_credencial()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("N", "n@x.com", "h", FCG.Domain.Constants.Roles.Usuario));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;
        var v0 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;

        var repo = new UsuarioRepository(db.Db);
        var admin = new AdminUserService(repo, db.Db);
        await admin.UpdateRoleAsync(userId, new UpdateUserRoleRequest(FCG.Domain.Constants.Roles.Administrador));

        var v1 = (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).CredencialVersao;
        v1.Should().BeGreaterThan(v0);
        (await db.Db.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId)).Perfil.Should()
            .Be(FCG.Domain.Constants.Roles.Administrador);
    }
}
