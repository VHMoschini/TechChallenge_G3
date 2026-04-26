using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/admin/promocoes")]
[Authorize(Roles = Roles.Administrador)]
public class AdminPromocoesController : ControllerBase
{
    private readonly IPromocaoService _promocoes;

    public AdminPromocoesController(IPromocaoService promocoes) => _promocoes = promocoes;

    [HttpPost]
    [ProducesResponseType(typeof(PromocaoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PromocaoResponse>> Create([FromBody] CreatePromocaoRequest request, CancellationToken cancellationToken)
    {
        var p = await _promocoes.CreateAsync(request, cancellationToken);
        return Ok(p);
    }
}
