using FCG.Domain.Services;
using FluentAssertions;

namespace FCG.Tests.Domain;

public class CredentialValidationTests
{
    [Theory]
    [InlineData("a@b.co")]
    [InlineData("user@example.com")]
    public void IsValidEmail_Aceita_formatos_comuns(string email) =>
        CredentialValidation.IsValidEmail(email).Should().BeTrue();

    [Theory]
    [InlineData("")]
    [InlineData("invalido")]
    [InlineData("@semusuario.com")]
    public void IsValidEmail_Rejeita_invalidos(string email) =>
        CredentialValidation.IsValidEmail(email).Should().BeFalse();

    [Fact]
    public void IsStrongPassword_Aceita_senha_conforme_enunciado()
    {
        CredentialValidation.IsStrongPassword("Abc@1234", out var err).Should().BeTrue();
        err.Should().BeNull();
    }

    [Theory]
    [InlineData("curta1!")]
    [InlineData("semnumero!A")]
    [InlineData("12345678!")]
    [InlineData("SemEspecial1")]
    public void IsStrongPassword_Rejeita_senhas_fracas(string password)
    {
        CredentialValidation.IsStrongPassword(password, out var err).Should().BeFalse();
        err.Should().NotBeNullOrEmpty();
    }
}
