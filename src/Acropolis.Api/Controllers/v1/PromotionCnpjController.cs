using Acropolis.Api.Extensions.Authentication;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acropolis.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/desconto-mais/cnpjs")]
public class PromotionCnpjController : ControllerBase
{
    private readonly IPromotionAppService _promotionAppService;
    private readonly IPromotionCnpjRepository _repository;

    public PromotionCnpjController(
        IPromotionCnpjRepository repository,
        IPromotionAppService promotionAppService)
    {
        _repository = repository;
        _promotionAppService = promotionAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PromotionCnpjResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePromotionCnpj(CreatePromotionCnpjRequest request)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var (response, createdPromotionCnpjDetails) = await _promotionAppService.AddCnpjsAsync(request, sellerId);

        if (!response.IsValid() || createdPromotionCnpjDetails is null)
            return UnprocessableEntity(response.ToValidationErrors());

        return Created($"/v1/desconto-mais/{createdPromotionCnpjDetails?.ExternalId}", createdPromotionCnpjDetails);        
    }

    [HttpGet("{externalId}")]
    [ProducesResponseType(typeof(IPagedList<PromotionCnpjResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> FindPromotions(string externalId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize=5)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var paginatedPromotionItems = await _repository.FindAsync(new PromotionCnpjParameters(){ ExternalId = externalId, PageIndex = pageIndex, PageSize = pageSize }, sellerId);

        return Ok(paginatedPromotionItems);
    }

    [HttpDelete("{externalId}/{cnpj}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RemovePromotionCnpj(string externalId, string cnpj)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var request = new RemovePromotionCnpjRequest(externalId, cnpj);

        var response = await _promotionAppService.RemoveCnpjAsync(request, sellerId);

        if (!response.IsValid())
            return UnprocessableEntity(response.ToValidationErrors());

        return NoContent();
    }
}
