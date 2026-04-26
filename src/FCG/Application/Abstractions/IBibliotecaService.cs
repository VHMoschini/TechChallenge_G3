using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IBibliotecaService
{
    Task AcquireAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GameResponse>> GetMyLibraryAsync(Guid userId, CancellationToken cancellationToken = default);
}
