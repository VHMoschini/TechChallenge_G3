using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IAdminUserService
{
    Task<IReadOnlyList<UserSummaryResponse>> ListUsersAsync(CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Guid userId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default);
    Task<UserSummaryResponse> UpdateUserAsync(Guid userId, UpdateUserAdminRequest request, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
