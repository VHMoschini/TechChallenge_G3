namespace FCG.Domain.Entities;

public class Jogo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Titulo { get; private set; } = string.Empty;
    public string Genero { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }

    private Jogo() { }

    public Jogo(string titulo, string genero, decimal preco)
    {
        Titulo = titulo;
        Genero = genero;
        Preco = preco;
    }
}
