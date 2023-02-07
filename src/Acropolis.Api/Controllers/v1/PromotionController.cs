using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Acropolis.Application.Base.Pagination;
using Acropolis.Api.Extensions.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Acropolis.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/desconto-mais")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionAppService _promotionAppService;
    private readonly IPromotionRepository _repository;

    public PromotionController(
        IPromotionRepository repository,
        IPromotionAppService promotionAppService)
    {
        _repository = repository;
        _promotionAppService = promotionAppService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IPagedList<PromotionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FindPromotions([FromQuery] PromotionParameters parameters)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var paginatedPromotionItems = await _repository.FindAsync(parameters, sellerId);

        return Ok(paginatedPromotionItems);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePromotion(CreatePromotionRequest request)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var (response, createdPromotion) = await _promotionAppService.CreatePromotionAsync(request, sellerId);

        if (!response.IsValid() || createdPromotion is null)
            return UnprocessableEntity(response.ToValidationErrors());

        var createdPromotionDetails = new PromotionResponse(createdPromotion);

        return Created($"/{createdPromotionDetails.ExternalId}", createdPromotionDetails);
    }

    [HttpGet("{externalId}")]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPromotionByExternalId(string externalId)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var (response, promotion) = await _promotionAppService.GetPromotionAsync(externalId, sellerId);

        if (!response.IsValid() || promotion is null)
            return NotFound(new Notification("Promotion", "Promotion Not Found"));

        return Ok(promotion);
    }

    [HttpDelete("{externalId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RemovePromotionAsync(string externalId)
    {
        var sellerId = Guid.TryParse(User.GetFirstSellerIdFromToken(), out var id) ? id : Guid.Empty;

        if (sellerId == Guid.Empty)
            return BadRequest("Seller Id inválido");

        var request = new RemovePromotionRequest(externalId);

        var response = await _promotionAppService.RemovePromotionAsync(request, sellerId);

        if (!response.IsValid())
            return UnprocessableEntity(response.ToValidationErrors());

        return NoContent();
    }
}
