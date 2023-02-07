using Acropolis.Api.Extensions.Authentication;
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
[Route("v{version:apiVersion}/desconto-mais/discount-limit")]
public class DiscountLimitController : ControllerBase
{
    private readonly IDiscountLimitAppService _discountLimitAppService;
    
    public DiscountLimitController(IDiscountLimitAppService discountLimitAppService)
    {
        _discountLimitAppService = discountLimitAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<DiscountLimitResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDiscountLimitAsync(IEnumerable<CreateDiscountLimitRequest> request)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var (response, createdDiscountLimitDetails) = await _discountLimitAppService.CreateDiscountLimitAsync(request, sellerId);

        if (!response.IsValid() || createdDiscountLimitDetails is null)
            return UnprocessableEntity(response.ToValidationErrors());

        return Created($"/v1/desconto-mais/discount-limit/", createdDiscountLimitDetails.Select(discountLimit => new DiscountLimitResponse(discountLimit)));
    }

    [HttpGet("{cnpj}")]
    [ProducesResponseType(typeof(DiscountLimitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDiscountLimitByCnpj(string cnpj)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var (response, discountLimit) = await _discountLimitAppService.GetDiscountLimitAsync(cnpj, sellerId);

        if (!response.IsValid())
            return NotFound(response.ToValidationErrors());

        return Ok(discountLimit);
    }

    [HttpDelete("{cnpj}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RemoveDiscountLimitAsync(string cnpj)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var request = new RemoveDiscountLimitRequest(cnpj);

        var response = await _discountLimitAppService.RemoveDiscountLimitAsync(request, sellerId);

        if (!response.IsValid())
            return UnprocessableEntity(response.ToValidationErrors());

        return NoContent();
    }
}
