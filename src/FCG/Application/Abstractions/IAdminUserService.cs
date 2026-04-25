using FCG.Application.Contracts;

namespace FCG.Application.Abstractions;

public interface IAdminUserService
{
    Task<IReadOnlyList<UserSummaryResponse>> ListUsersAsync(CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Guid userId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default);
}
