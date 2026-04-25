namespace FCG.Domain.Entities;

public class Promocao
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public DateTime ValidFromUtc { get; private set; }
    public DateTime ValidToUtc { get; private set; }
    public Guid? GameId { get; private set; }

    private Promocao() { }

    public Promocao(string title, string? description, decimal discountPercent, DateTime validFromUtc, DateTime validToUtc, Guid? gameId = null)
    {
        Title = title;
        Description = description;
        DiscountPercent = discountPercent;
        ValidFromUtc = validFromUtc;
        ValidToUtc = validToUtc;
        GameId = gameId;
    }
}
