using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Services;

public class PromocaoService : IPromocaoService
{
    private readonly AppDbContext _db;

    public PromocaoService(AppDbContext db) => _db = db;

    public async Task<PromocaoResponse> CreateAsync(CreatePromocaoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
            throw new InvalidOperationException("Titulo e obrigatorio.");
        if (request.PercentualDesconto is < 0 or > 100)
            throw new InvalidOperationException("Desconto deve estar entre 0 e 100.");
        if (request.DataPromoInicio >= request.DataPromoFim)
            throw new InvalidOperationException("Datas de vigencia invalidas.");

        if (request.GameId is { } gid && !await _db.Jogos.AnyAsync(g => g.Id == gid, cancellationToken))
            throw new InvalidOperationException("Jogo informado nao existe.");

        var p = new Promocao(
            request.Titulo.Trim(),
            request.Descricao?.Trim(),
            request.PercentualDesconto,
            request.DataPromoInicio,
            request.DataPromoFim,
            request.GameId);

        _db.Promocoes.Add(p);
        await _db.SaveChangesAsync(cancellationToken);

        return new PromocaoResponse(p.Id, p.Titulo, p.Descricao, p.PercentualDisconto, p.DataPromoInicio, p.DataPromoFim, p.GameId);
    }

    public async Task<IReadOnlyList<PromocaoResponse>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var list = await _db.Promocoes.AsNoTracking()
            .Where(p => p.DataPromoInicio <= now && now <= p.DataPromoFim)
            .OrderBy(p => p.Titulo)
            .Select(p => new PromocaoResponse(p.Id, p.Titulo, p.Descricao, p.PercentualDisconto, p.DataPromoInicio, p.DataPromoFim, p.GameId))
            .ToListAsync(cancellationToken);
        return list;
    }
}
