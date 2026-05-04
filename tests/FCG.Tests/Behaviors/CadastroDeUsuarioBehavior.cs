using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Security;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FCG.Tests.Behaviors;

// BDD - Modulo: Cadastro e Autenticacao de Usuarios (FCG Fase 1).
// Cada teste segue o ritual Dado / Quando / Entao para tornar a regra de negocio
// auto-explicativa, conforme o requisito "TDD/BDD em pelo menos um modulo".
public class CadastroDeUsuarioBehavior
{
    private static IJwtTokenGenerator JwtGenerator() =>
        new JwtTokenGenerator(Options.Create(new JwtOptions
        {
            Key = "FCG_CHAVE_LOCAL_MINIMO_32_CARACTERES_PARA_HS256_SEGURO",
            Issuer = "FCG",
            Audience = "FCG",
            ExpiresMinutes = 60
        }));

    private static AuthService NovaAutenticacao(TestDatabase db) =>
        new(new UsuarioRepository(db.Db), JwtGenerator());

    [Fact(DisplayName = "Dado dados validos, quando registro um usuario, entao recebo token JWT e o perfil padrao 'Usuario'")]
    public async Task Cenario_registro_com_dados_validos()
    {
        // Dado: um banco vazio e um servico de autenticacao
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        // Quando: registro com nome, e-mail valido e senha forte
        var resposta = await auth.RegisterAsync(new RegisterRequest("Alice", "Alice@Email.com", "Abc@1234"));

        // Entao: recebo um AuthResponse com token, perfil padrao e e-mail normalizado
        resposta.Token.Should().NotBeNullOrWhiteSpace();
        resposta.Role.Should().Be(Roles.Usuario);
        resposta.Email.Should().Be("alice@email.com");
        resposta.Name.Should().Be("Alice");
        resposta.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Theory(DisplayName = "Dado um e-mail invalido, quando tento registrar, entao a operacao e rejeitada")]
    [InlineData("")]
    [InlineData("semarroba")]
    [InlineData("@semusuario.com")]
    public async Task Cenario_registro_rejeita_email_invalido(string email)
    {
        // Dado
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        // Quando
        var act = async () => await auth.RegisterAsync(new RegisterRequest("Alice", email, "Abc@1234"));

        // Entao
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*E-mail*");
    }

    [Theory(DisplayName = "Dado uma senha que viola a politica, quando tento registrar, entao a operacao e rejeitada")]
    [InlineData("curta1!")]            // < 8 caracteres
    [InlineData("semnumero!A")]        // sem digito
    [InlineData("12345678!")]          // sem letra
    [InlineData("SemEspecial1")]       // sem caractere especial
    public async Task Cenario_registro_rejeita_senha_fraca(string senha)
    {
        // Dado
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        // Quando
        var act = async () => await auth.RegisterAsync(new RegisterRequest("Alice", "alice@email.com", senha));

        // Entao
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Senha*");
    }

    [Fact(DisplayName = "Dado um nome em branco, quando tento registrar, entao a operacao e rejeitada")]
    public async Task Cenario_registro_rejeita_nome_em_branco()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        var act = async () => await auth.RegisterAsync(new RegisterRequest("   ", "alice@email.com", "Abc@1234"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Nome*");
    }

    [Fact(DisplayName = "Dado um e-mail ja cadastrado e ativo, quando tento registrar de novo, entao recebo erro de duplicidade")]
    public async Task Cenario_registro_rejeita_email_duplicado()
    {
        // Dado: usuario ja existe ativo
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);
        await auth.RegisterAsync(new RegisterRequest("Alice", "dup@email.com", "Abc@1234"));

        // Quando
        var act = async () => await auth.RegisterAsync(new RegisterRequest("Outra", "dup@email.com", "Abc@1234"));

        // Entao
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*ja cadastrado*");
    }

    [Fact(DisplayName = "Dado credenciais corretas, quando faco login, entao recebo token com expiracao no futuro")]
    public async Task Cenario_login_com_credenciais_corretas()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);
        await auth.RegisterAsync(new RegisterRequest("Bob", "bob@email.com", "Abc@1234"));

        var resposta = await auth.LoginAsync(new LoginRequest("BOB@email.com", "Abc@1234"));

        resposta.Token.Should().NotBeNullOrWhiteSpace();
        resposta.UserId.Should().NotBeEmpty();
        resposta.Email.Should().Be("bob@email.com");
        resposta.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact(DisplayName = "Dado uma senha incorreta, quando faco login, entao recebo Unauthorized")]
    public async Task Cenario_login_rejeita_senha_incorreta()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);
        await auth.RegisterAsync(new RegisterRequest("Bob", "bob@email.com", "Abc@1234"));

        var act = async () => await auth.LoginAsync(new LoginRequest("bob@email.com", "Errada@9999"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Credenciais*");
    }

    [Fact(DisplayName = "Dado um e-mail nao cadastrado, quando faco login, entao recebo Unauthorized generico (nao revela existencia)")]
    public async Task Cenario_login_rejeita_email_inexistente_sem_revelar_existencia()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        var act = async () => await auth.LoginAsync(new LoginRequest("nao@existe.com", "Abc@1234"));

        // mesma mensagem da senha errada -> evita user enumeration
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Credenciais*");
    }

    [Fact(DisplayName = "Dado um usuario inativado, quando ele tenta logar, entao o login e bloqueado")]
    public async Task Cenario_login_bloqueia_usuario_inativo()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);
        var registrado = await auth.RegisterAsync(new RegisterRequest("Carla", "carla@email.com", "Abc@1234"));
        var carla = await db.Db.Usuarios.FirstAsync(u => u.Id == registrado.UserId);
        carla.Inativar();
        await db.Db.SaveChangesAsync();

        var act = async () => await auth.LoginAsync(new LoginRequest("carla@email.com", "Abc@1234"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "Dado um e-mail com letras maiusculas, quando registro, entao o e-mail persistido fica normalizado em minusculas")]
    public async Task Cenario_registro_normaliza_email_para_lowercase()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        await auth.RegisterAsync(new RegisterRequest("Diana", "  Diana@FIAP.COM ", "Abc@1234"));

        var persistido = await db.Db.Usuarios.AsNoTracking().FirstAsync();
        persistido.Email.Should().Be("diana@fiap.com");
    }

    [Fact(DisplayName = "Dado um usuario novo, quando registro, entao a senha persistida e um hash BCrypt (nunca em texto puro)")]
    public async Task Cenario_registro_persiste_senha_em_hash_bcrypt()
    {
        await using var db = await TestDatabase.CreateAsync();
        var auth = NovaAutenticacao(db);

        await auth.RegisterAsync(new RegisterRequest("Eve", "eve@email.com", "Abc@1234"));

        var persistido = await db.Db.Usuarios.AsNoTracking().FirstAsync();
        persistido.SenhaHash.Should().NotBe("Abc@1234");
        BCrypt.Net.BCrypt.Verify("Abc@1234", persistido.SenhaHash).Should().BeTrue();
    }
}
