namespace FCG.Domain.Entities;

public class UsuarioJogo
{
    public Guid UsuarioId { get; private set; }
    public Guid JogoId { get; private set; }
    public DateTime AcquiredAtUtc { get; private set; }

    private UsuarioJogo() { }

    public UsuarioJogo(Guid usuarioId, Guid jogoId)
    {
        UsuarioId = usuarioId;
        JogoId = jogoId;
        AcquiredAtUtc = DateTime.UtcNow;
    }
}
