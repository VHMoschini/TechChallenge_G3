namespace FCG.Infrastructure.Security;

public static class JwtCustomClaims
{
    /// <summary>Versao da credencial no usuario; incremento invalida JWTs antigos.</summary>
    public const string CredencialVersao = "cred_v";
}
