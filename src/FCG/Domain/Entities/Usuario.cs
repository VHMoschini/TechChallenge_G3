using FCG.Domain.Constants;

namespace FCG.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Perfil { get; private set; } = Roles.Usuario;
    public int CredencialVersao { get; private set; } = 1;

    private Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string perfil = Roles.Usuario)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
        CredencialVersao = 1;
    }

    public void DefinirPerfil(string perfil)
    {
        if (!Roles.IsValid(perfil))
            throw new ArgumentException("Perfil invalido.", nameof(perfil));
        Perfil = perfil;
    }

    public void AtualizarDados(string nome, string emailNormalizado)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome invalido.", nameof(nome));
        if (string.IsNullOrWhiteSpace(emailNormalizado))
            throw new ArgumentException("E-mail invalido.", nameof(emailNormalizado));
        Nome = nome.Trim();
        Email = emailNormalizado;
    }

    public void DefinirSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Hash de senha invalido.", nameof(senhaHash));
        SenhaHash = senhaHash;
    }

    /// <summary>Incrementa a versao para invalidar JWTs emitidos antes desta rotacao.</summary>
    public void RotacionarCredencialJwt()
    {
        CredencialVersao = CredencialVersao == int.MaxValue ? 1 : CredencialVersao + 1;
    }
}
