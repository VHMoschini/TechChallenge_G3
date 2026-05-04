using FCG.Domain.Constants;
using FluentAssertions;

namespace FCG.Tests.Domain;

public class RolesTests
{
    [Theory]
    [InlineData(Roles.Usuario)]
    [InlineData(Roles.Administrador)]
    public void IsValid_aceita_perfis_conhecidos(string perfil) =>
        Roles.IsValid(perfil).Should().BeTrue();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("admin")]      // case-sensitive: nao bate com "Administrador"
    [InlineData("USUARIO")]
    [InlineData("Visitante")]
    public void IsValid_rejeita_valores_desconhecidos(string? perfil) =>
        Roles.IsValid(perfil).Should().BeFalse();
}
