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
}
