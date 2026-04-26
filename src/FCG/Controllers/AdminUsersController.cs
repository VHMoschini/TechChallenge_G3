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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateRole(Guid userId, [FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        await _admin.UpdateRoleAsync(userId, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserSummaryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSummaryResponse>> UpdateUser(Guid userId, [FromBody] UpdateUserAdminRequest request, CancellationToken cancellationToken)
    {
        var user = await _admin.UpdateUserAsync(userId, request, cancellationToken);
        return Ok(user);
    }
}
