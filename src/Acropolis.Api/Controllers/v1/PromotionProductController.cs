using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acropolis.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/desconto-mais/incentivelist")]
public class PromotionProductController : ControllerBase
{
    private readonly IPromotionProductAppService _promotionProductAppService;

    public PromotionProductController(IPromotionProductAppService promotionProductAppService)
    {
        _promotionProductAppService = promotionProductAppService;
    }

    [HttpGet("{productId}/{sellerId}/{cnpj}")]
    [ProducesResponseType(typeof(PromotionProductIncentiveListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPromotionProductIncentiveList(string productId, string sellerId, string cnpj, [FromQuery] int pageSize = 50)
    {
        var parameters = new GetPromotionProductIncentiveListRequest(productId, sellerId, cnpj, pageSize);
        var (response, promotionProduct) = await _promotionProductAppService.GetProductIncentiveListAsync(parameters);

        if (!response.IsValid() || promotionProduct is null)
            return NotFound(response.ToValidationErrors());

        return Ok(promotionProduct);
    }
}