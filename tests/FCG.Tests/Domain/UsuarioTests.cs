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
}
