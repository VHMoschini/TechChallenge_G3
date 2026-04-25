using System.Text.RegularExpressions;

namespace FCG.Domain.Services;

/// <summary>
/// Regras do enunciado: e-mail valido; senha minimo 8 caracteres com letras, numeros e caracteres especiais.
/// </summary>
public static class CredentialValidation
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email.Trim());

    public static bool IsStrongPassword(string? password, out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Senha e obrigatoria.";
            return false;
        }

        if (password.Length < 8)
        {
            errorMessage = "Senha deve ter no minimo 8 caracteres.";
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Za-z\u00C0-\u024F]"))
        {
            errorMessage = "Senha deve conter pelo menos uma letra.";
            return false;
        }

        if (!Regex.IsMatch(password, @"\d"))
        {
            errorMessage = "Senha deve conter pelo menos um numero.";
            return false;
        }

        if (!Regex.IsMatch(password, @"[^A-Za-z0-9\u00C0-\u024F]"))
        {
            errorMessage = "Senha deve conter pelo menos um caractere especial.";
            return false;
        }

        return true;
    }
}
