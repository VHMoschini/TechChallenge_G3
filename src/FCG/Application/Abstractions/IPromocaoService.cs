using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IPromocaoService
{
    Task<PromocaoResponse> CreateAsync(CreatePromocaoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromocaoResponse>> ListActiveAsync(CancellationToken cancellationToken = default);
}
