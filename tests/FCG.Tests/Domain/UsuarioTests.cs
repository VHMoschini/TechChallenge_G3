using System.Reflection;
using FCG.Domain.Constants;
using FCG.Domain.Entities;
using FluentAssertions;

namespace FCG.Tests.Domain;

public class UsuarioTests
{
    [Fact]
    public void Deve_criar_usuario_com_perfil_Usuario_padrao()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");

        usuario.Perfil.Should().Be("Usuario");
    }

    [Fact]
    public void Novo_usuario_tem_credencial_versao_1()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.CredencialVersao.Should().Be(1);
    }

    [Fact]
    public void RotacionarCredencialJwt_incrementa_versao()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.RotacionarCredencialJwt();
        usuario.CredencialVersao.Should().Be(2);
    }

    [Fact]
    public void Novo_usuario_esta_ativo()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Inativar_desativa_e_rotaciona_credencial()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        var v0 = usuario.CredencialVersao;
        usuario.Inativar();
        usuario.Ativo.Should().BeFalse();
        usuario.CredencialVersao.Should().BeGreaterThan(v0);
    }

    [Fact]
    public void Inativar_em_usuario_ja_inativo_nao_rotaciona_de_novo()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.Inativar();
        var depoisDaPrimeira = usuario.CredencialVersao;

        usuario.Inativar();

        usuario.CredencialVersao.Should().Be(depoisDaPrimeira);
        usuario.Ativo.Should().BeFalse();
    }

    [Fact]
    public void DefinirPerfil_aceita_perfil_valido()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.DefinirPerfil(Roles.Administrador);
        usuario.Perfil.Should().Be(Roles.Administrador);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("")]
    public void DefinirPerfil_rejeita_perfil_invalido(string perfilInvalido)
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        var act = () => usuario.DefinirPerfil(perfilInvalido);
        act.Should().Throw<ArgumentException>().WithParameterName("perfil");
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    public void AtualizarDados_rejeita_nome_em_branco(string nome)
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        var act = () => usuario.AtualizarDados(nome, "novo@email.com");
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    [Fact]
    public void AtualizarDados_rejeita_email_em_branco()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        var act = () => usuario.AtualizarDados("Alice", " ");
        act.Should().Throw<ArgumentException>().WithParameterName("emailNormalizado");
    }

    [Fact]
    public void AtualizarDados_remove_espacos_extra_do_nome()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        usuario.AtualizarDados("  Alice Cooper  ", "novo@email.com");
        usuario.Nome.Should().Be("Alice Cooper");
        usuario.Email.Should().Be("novo@email.com");
    }

    [Fact]
    public void DefinirSenhaHash_rejeita_hash_em_branco()
    {
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        var act = () => usuario.DefinirSenhaHash("   ");
        act.Should().Throw<ArgumentException>().WithParameterName("senhaHash");
    }

    [Fact]
    public void RotacionarCredencialJwt_no_overflow_volta_para_1()
    {
        // Forca CredencialVersao = int.MaxValue via reflection para validar a regra de overflow.
        var usuario = new Usuario("Alice", "alice@email.com", "hash");
        typeof(Usuario)
            .GetProperty(nameof(Usuario.CredencialVersao), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(usuario, int.MaxValue);

        usuario.RotacionarCredencialJwt();

        usuario.CredencialVersao.Should().Be(1);
    }
}
