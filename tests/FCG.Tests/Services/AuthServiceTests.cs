using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Security;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FCG.Tests.Services;

public class AuthServiceTests
{
    private static IJwtTokenGenerator CreateJwt()
    {
        var opt = Options.Create(new JwtOptions
        {
            Key = "FCG_CHAVE_LOCAL_MINIMO_32_CARACTERES_PARA_HS256_SEGURO",
            Issuer = "FCG",
            Audience = "FCG",
            ExpiresMinutes = 60
        });
        return new JwtTokenGenerator(opt);
    }

    private static AuthService CreateAuth(TestDatabase db)
    {
        var repo = new UsuarioRepository(db.Db);
        return new AuthService(repo, CreateJwt());
    }

    [Fact]
    public async Task LoginAsync_usuario_inativo_lanca_UnauthorizedAccessException()
    {
        await using var db = await TestDatabase.CreateAsync();
        var usuario = new Usuario("U", "u@test.com", BCrypt.Net.BCrypt.HashPassword("Abc@1234"));
        db.Db.Usuarios.Add(usuario);
        await db.Db.SaveChangesAsync();
        usuario.Inativar();
        await db.Db.SaveChangesAsync();

        var auth = CreateAuth(db);
        var act = async () => await auth.LoginAsync(new LoginRequest("u@test.com", "Abc@1234"));
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task RegisterAsync_permite_mesmo_email_de_usuario_inativo()
    {
        await using var db = await TestDatabase.CreateAsync();
        var antigo = new Usuario("Antigo", "dup@test.com", BCrypt.Net.BCrypt.HashPassword("Abc@1234"));
        db.Db.Usuarios.Add(antigo);
        await db.Db.SaveChangesAsync();
        antigo.Inativar();
        await db.Db.SaveChangesAsync();

        var auth = CreateAuth(db);
        var res = await auth.RegisterAsync(new RegisterRequest("Novo", "dup@test.com", "Abc@1234"));

        res.Email.Should().Be("dup@test.com");
        res.Name.Should().Be("Novo");

        var ativos = await db.Db.Usuarios.AsNoTracking().Where(u => u.Email == "dup@test.com" && u.Ativo).ToListAsync();
        ativos.Should().ContainSingle();
    }
}
