namespace FCG.Domain.Constants;

public static class Roles
{
    public const string Usuario = "Usuario";
    public const string Administrador = "Administrador";

    public static bool IsValid(string? role) =>
        role == Usuario || role == Administrador;
}
