using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = Roles.Administrador)]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _admin;

    public AdminUsersController(IAdminUserService admin) => _admin = admin;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserSummaryResponse>>> List(CancellationToken cancellationToken)
    {
        var users = await _admin.ListUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPatch("{userId:guid}/role")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> UpdateRole(Guid userId, [FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        await _admin.UpdateRoleAsync(userId, request, cancellationToken);
        return Ok(new ApiMessageResponse("Perfil do usuario atualizado."));
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserSummaryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSummaryResponse>> UpdateUser(Guid userId, [FromBody] UpdateUserAdminRequest request, CancellationToken cancellationToken)
    {
        var user = await _admin.UpdateUserAsync(userId, request, cancellationToken);
        return Ok(user);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> Deactivate(Guid userId, CancellationToken cancellationToken)
    {
        await _admin.DeactivateUserAsync(userId, cancellationToken);
        return Ok(new ApiMessageResponse("Usuario inativado com sucesso."));
    }

    [HttpPost("{userId:guid}/reactivate")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> Reactivate(Guid userId, CancellationToken cancellationToken)
    {
        await _admin.ReactivateUserAsync(userId, cancellationToken);
        return Ok(new ApiMessageResponse("Usuario reativado com sucesso."));
    }
}
