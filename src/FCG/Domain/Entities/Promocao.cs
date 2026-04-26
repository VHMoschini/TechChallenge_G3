namespace FCG.Domain.Entities;

public class Promocao
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public decimal PercentualDisconto { get; private set; }
    public DateTime DataPromoInicio { get; private set; }
    public DateTime DataPromoFim { get; private set; }
    public Guid? GameId { get; private set; }

    private Promocao() { }

    public Promocao(string titulo, string? descricao, decimal percentualDisconto, DateTime dataPromoInicio, DateTime dataPromoFim, Guid? gameId = null)
    {
        Titulo = titulo;
        Descricao = descricao;
        PercentualDisconto = percentualDisconto;
        DataPromoInicio = dataPromoInicio;
        DataPromoFim = dataPromoFim;
        GameId = gameId;
    }
}
