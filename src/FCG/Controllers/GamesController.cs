using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _games;

    public GamesController(IGameService games) => _games = games;

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GameResponse>> Create([FromBody] CreateGameRequest request, CancellationToken cancellationToken)
    {
        var game = await _games.CreateAsync(request, cancellationToken);
        return Ok(game);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GameResponse>> Update(Guid id, [FromBody] UpdateGameRequest request, CancellationToken cancellationToken)
    {
        var game = await _games.UpdateAsync(id, request, cancellationToken);
        return Ok(game);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> List(CancellationToken cancellationToken)
    {
        var list = await _games.ListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiMessageResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _games.DeactivateAsync(id, cancellationToken);
        return Ok(new ApiMessageResponse("Jogo inativado com sucesso."));
    }

    [HttpPost("{id:guid}/reactivate")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GameResponse>> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        var game = await _games.ReactivateAsync(id, cancellationToken);
        return Ok(game);
    }
}
