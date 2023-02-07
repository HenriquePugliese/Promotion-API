using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acropolis.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
//[Authorize]
[Route("v{version:apiVersion}/desconto-mais/promotion-order")]
public class PromotionOrderController : ControllerBase
{
    private readonly IPromotionDiscountAppService _promotionDiscountAppService;
    
    public PromotionOrderController(IPromotionDiscountAppService promotionDiscountAppService)
    {
        _promotionDiscountAppService = promotionDiscountAppService;
    }

    [HttpPost(Name = "get-promotion-order")]
    [ProducesResponseType(typeof(PromotionOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]    
    public async Task<IActionResult> GetPromotionOrder(PromotionOrderRequest request)
    {
        var (response, promotionOrder) = await _promotionDiscountAppService.GetPromotionOrderAsync(request);

        if (!response.IsValid() || promotionOrder is null)
            return NotFound(response.ToValidationErrors());

        return Ok(promotionOrder);
    }
}