using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/admin/promotions")]
[Authorize(Roles = Roles.Administrador)]
public class AdminPromotionsController : ControllerBase
{
    private readonly IPromotionService _promotions;

    public AdminPromotionsController(IPromotionService promotions) => _promotions = promotions;

    [HttpPost]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PromotionResponse>> Create([FromBody] CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        var p = await _promotions.CreateAsync(request, cancellationToken);
        return Ok(p);
    }
}
