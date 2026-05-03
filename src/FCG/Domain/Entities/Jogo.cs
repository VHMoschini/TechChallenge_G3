namespace FCG.Domain.Entities;

public class Jogo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Titulo { get; private set; } = string.Empty;
    public string Genero { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public bool Ativo { get; private set; } = true;

    private Jogo() { }

    public Jogo(string titulo, string genero, decimal preco)
    {
        Titulo = titulo;
        Genero = genero;
        Preco = preco;
        Ativo = true;
    }

    public void Inativar() => Ativo = false;

    public void Reativar() => Ativo = true;

    public void Atualizar(string titulo, string genero, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Titulo invalido.", nameof(titulo));
        if (preco < 0)
            throw new ArgumentException("Preco invalido.", nameof(preco));
        Titulo = titulo.Trim();
        Genero = genero?.Trim() ?? string.Empty;
        Preco = preco;
    }
}
