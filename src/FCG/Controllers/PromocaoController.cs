using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/promocoes")]
public class PromocoesController : ControllerBase
{
    private readonly IPromocaoService _promocoes;

    public PromocoesController(IPromocaoService promocoes) => _promocoes = promocoes;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PromocaoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PromocaoResponse>>> ListActive(CancellationToken cancellationToken)
    {
        var list = await _promocoes.ListActiveAsync(cancellationToken);
        return Ok(list);
    }
}
