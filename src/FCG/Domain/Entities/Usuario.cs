using FCG.Domain.Constants;

namespace FCG.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Perfil { get; private set; } = Roles.Usuario;

    private Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string perfil = Roles.Usuario)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
    }

    public void DefinirPerfil(string perfil)
    {
        if (!Roles.IsValid(perfil))
            throw new ArgumentException("Perfil invalido.", nameof(perfil));
        Perfil = perfil;
    }
}
