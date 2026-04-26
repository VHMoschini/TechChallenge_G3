using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IUserProfileService
{
    Task<UserSummaryResponse> UpdateMyProfileAsync(Guid userId, UpdateMyProfileRequest request, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
