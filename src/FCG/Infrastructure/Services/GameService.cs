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
        if (string.IsNullOrWhiteSpace(request.Titulo))
            throw new InvalidOperationException("Titulo e obrigatorio.");
        if (request.Preco < 0)
            throw new InvalidOperationException("Preco invalido.");

        var genero = request.Genero?.Trim() ?? string.Empty;
        var jogo = new Jogo(request.Titulo.Trim(), genero, request.Preco);
        _db.Jogos.Add(jogo);
        await _db.SaveChangesAsync(cancellationToken);
        return new GameResponse(jogo.Id, jogo.Titulo, jogo.Genero, jogo.Preco);
    }

    public async Task<GameResponse> UpdateAsync(Guid id, UpdateGameRequest request, CancellationToken cancellationToken = default)
    {
        var jogo = await _db.Jogos.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (jogo is null)
            throw new KeyNotFoundException("Jogo nao encontrado.");

        jogo.Atualizar(request.Titulo, request.Genero, request.Preco);
        await _db.SaveChangesAsync(cancellationToken);
        return new GameResponse(jogo.Id, jogo.Titulo, jogo.Genero, jogo.Preco);
    }

    public async Task<IReadOnlyList<GameResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Jogos.AsNoTracking()
            .OrderBy(g => g.Titulo)
            .Select(g => new GameResponse(g.Id, g.Titulo, g.Genero, g.Preco))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogo = await _db.Jogos.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (jogo is null)
            throw new KeyNotFoundException("Jogo nao encontrado.");
        jogo.Inativar();
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<GameResponse> ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogo = await _db.Jogos.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (jogo is null)
            throw new KeyNotFoundException("Jogo nao encontrado.");
        jogo.Reativar();
        await _db.SaveChangesAsync(cancellationToken);
        return new GameResponse(jogo.Id, jogo.Titulo, jogo.Genero, jogo.Preco);
    }
}
