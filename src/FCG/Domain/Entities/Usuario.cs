using FCG.Domain.Constants;

namespace FCG.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = Roles.Usuario;

    private Usuario() { }

    public Usuario(string name, string email, string passwordHash, string role = Roles.Usuario)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void SetRole(string role)
    {
        if (!Roles.IsValid(role))
            throw new ArgumentException("Perfil invalido.", nameof(role));
        Role = role;
    }
}
