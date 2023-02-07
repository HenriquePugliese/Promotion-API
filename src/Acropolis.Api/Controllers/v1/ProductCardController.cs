using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acropolis.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/desconto-mais/card")]
public class ProductCardController : ControllerBase
{
    private readonly IProductCardAppService _productCardAppService;
    public ProductCardController(IProductCardAppService productCardAppService)
    {
        _productCardAppService = productCardAppService;
    }

    [HttpPost(Name = "get-card")]
    [ProducesResponseType(typeof(IEnumerable<ProductCardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductHasPromotionAsync(ProductCardRequest request)
    {
        var response = await _productCardAppService.ProductListHasPromotionAsync(request);

        return Ok(response);
    }
}
