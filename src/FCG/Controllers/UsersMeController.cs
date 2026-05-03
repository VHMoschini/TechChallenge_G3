using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/users/me")]
[Authorize]
public class UsersMeController : ControllerBase
{
    private readonly IBibliotecaService _biblioteca;
    private readonly IUserProfileService _profile;

    public UsersMeController(IBibliotecaService biblioteca, IUserProfileService profile)
    {
        _biblioteca = biblioteca;
        _profile = profile;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProfile()
    {
        var id = User.GetUserId();
        var name = User.Identity?.Name;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var role = User.FindFirstValue("role")
            ?? User.FindFirstValue(ClaimTypes.Role)
            ?? User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        return Ok(new { id, name, email, role });
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserSummaryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSummaryResponse>> UpdateProfile([FromBody] UpdateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var updated = await _profile.UpdateMyProfileAsync(userId, request, cancellationToken);
        return Ok(updated);
    }

    [HttpPut("password")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _profile.ChangePasswordAsync(userId, request, cancellationToken);
        return Ok(new ApiMessageResponse("Senha alterada com sucesso."));
    }

    [HttpGet("biblioteca")]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> GetLibrary(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var list = await _biblioteca.GetMyLibraryAsync(userId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("biblioteca/games/{gameId:guid}")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> AcquireGame(Guid gameId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _biblioteca.AcquireAsync(userId, gameId, cancellationToken);
        return Ok(new ApiMessageResponse("Jogo adquirido e adicionado à sua biblioteca."));
    }

    [HttpDelete("biblioteca/games/{gameId:guid}")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> RemoveGame(Guid gameId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _biblioteca.RemoveAsync(userId, gameId, cancellationToken);
        return Ok(new ApiMessageResponse("Jogo removido da sua biblioteca."));
    }
}
