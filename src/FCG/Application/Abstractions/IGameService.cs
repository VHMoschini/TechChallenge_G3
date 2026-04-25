using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IGameService
{
    Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GameResponse>> ListAsync(CancellationToken cancellationToken = default);
}
