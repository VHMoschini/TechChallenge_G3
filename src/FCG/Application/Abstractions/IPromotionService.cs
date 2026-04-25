using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IPromotionService
{
    Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromotionResponse>> ListActiveAsync(CancellationToken cancellationToken = default);
}
