using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Services;

public class BibliotecaService : IBibliotecaService
{
    private readonly AppDbContext _db;

    public BibliotecaService(AppDbContext db) => _db = db;

    public async Task AcquireAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default)
    {
        var gameExists = await _db.Jogos.AnyAsync(g => g.Id == gameId, cancellationToken);
        if (!gameExists)
            throw new KeyNotFoundException("Jogo nao encontrado.");

        var already = await _db.UsuarioJogos.AnyAsync(
            uj => uj.UsuarioId == userId && uj.JogoId == gameId,
            cancellationToken);
        if (already)
            throw new InvalidOperationException("Jogo ja esta na sua biblioteca.");

        _db.UsuarioJogos.Add(new UsuarioJogo(userId, gameId));
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GameResponse>> GetMyLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var list = await _db.UsuarioJogos.AsNoTracking()
            .Where(uj => uj.UsuarioId == userId)
            .Join(_db.Jogos, uj => uj.JogoId, j => j.Id, (uj, j) => j)
            .OrderBy(j => j.Titulo)
            .Select(j => new GameResponse(j.Id, j.Titulo, j.Genero, j.Preco))
            .ToListAsync(cancellationToken);
        return list;
    }
}
