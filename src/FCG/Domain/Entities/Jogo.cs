namespace FCG.Domain.Entities;

public class Jogo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Genre { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private Jogo() { }

    public Jogo(string title, string genre, decimal price)
    {
        Title = title;
        Genre = genre;
        Price = price;
    }
}
