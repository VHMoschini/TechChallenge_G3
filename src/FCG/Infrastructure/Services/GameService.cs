using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _db;

    public GameService(AppDbContext db) => _db = db;

    public async Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new InvalidOperationException("Titulo e obrigatorio.");
        if (request.Price < 0)
            throw new InvalidOperationException("Preco invalido.");

        var genre = request.Genre?.Trim() ?? string.Empty;
        var jogo = new Jogo(request.Title.Trim(), genre, request.Price);
        _db.Jogos.Add(jogo);
        await _db.SaveChangesAsync(cancellationToken);
        return new GameResponse(jogo.Id, jogo.Title, jogo.Genre, jogo.Price);
    }

    public async Task<IReadOnlyList<GameResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Jogos.AsNoTracking()
            .OrderBy(g => g.Title)
            .Select(g => new GameResponse(g.Id, g.Title, g.Genre, g.Price))
            .ToListAsync(cancellationToken);
        return list;
    }
}
