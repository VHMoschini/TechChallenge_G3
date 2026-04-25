using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotions;

    public PromotionsController(IPromotionService promotions) => _promotions = promotions;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PromotionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PromotionResponse>>> ListActive(CancellationToken cancellationToken)
    {
        var list = await _promotions.ListActiveAsync(cancellationToken);
        return Ok(list);
    }
}
