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
    private readonly ILibraryService _library;

    public UsersMeController(ILibraryService library) => _library = library;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProfile()
    {
        var id = User.GetUserId();
        var name = User.Identity?.Name;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var role = User.FindFirstValue("role");
        return Ok(new { id, name, email, role });
    }

    [HttpGet("library")]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> GetLibrary(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var list = await _library.GetMyLibraryAsync(userId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("library/games/{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AcquireGame(Guid gameId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _library.AcquireAsync(userId, gameId, cancellationToken);
        return NoContent();
    }
}
