using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Services;

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _db;

    public PromotionService(AppDbContext db) => _db = db;

    public async Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new InvalidOperationException("Titulo e obrigatorio.");
        if (request.DiscountPercent is < 0 or > 100)
            throw new InvalidOperationException("Desconto deve estar entre 0 e 100.");
        if (request.ValidFromUtc >= request.ValidToUtc)
            throw new InvalidOperationException("Datas de vigencia invalidas.");

        if (request.GameId is { } gid && !await _db.Jogos.AnyAsync(g => g.Id == gid, cancellationToken))
            throw new InvalidOperationException("Jogo informado nao existe.");

        var p = new Promocao(
            request.Title.Trim(),
            request.Description?.Trim(),
            request.DiscountPercent,
            request.ValidFromUtc,
            request.ValidToUtc,
            request.GameId);

        _db.Promocoes.Add(p);
        await _db.SaveChangesAsync(cancellationToken);

        return new PromotionResponse(p.Id, p.Title, p.Description, p.DiscountPercent, p.ValidFromUtc, p.ValidToUtc, p.GameId);
    }

    public async Task<IReadOnlyList<PromotionResponse>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var list = await _db.Promocoes.AsNoTracking()
            .Where(p => p.ValidFromUtc <= now && now <= p.ValidToUtc)
            .OrderBy(p => p.Title)
            .Select(p => new PromotionResponse(p.Id, p.Title, p.Description, p.DiscountPercent, p.ValidFromUtc, p.ValidToUtc, p.GameId))
            .ToListAsync(cancellationToken);
        return list;
    }
}
