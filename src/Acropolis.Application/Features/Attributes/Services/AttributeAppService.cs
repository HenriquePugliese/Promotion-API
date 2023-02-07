using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Acropolis.Application.Features.Attributes.Services;

public class AttributeAppService : AppService, IAttributeAppService
{
    private readonly IAttributeRepository _repository;
    private readonly ILogger<AttributeAppService> _logger;

    public AttributeAppService(
        IUnitOfWork unitOfWork,
        IAttributeRepository repository,
        ILogger<AttributeAppService> logger)
        : base(unitOfWork)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<(Response, Attribute?)> CreateAttributeConsumer(CreateAttributeRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Create new attribute ({request.AttributeKeyId})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        var attributeDb = await _repository.FindByAsync(x =>
                                                    x.ProductId == request.ProductId &&
                                                    x.AttributeKey == request.AttributeKey &&
                                                    x.AttributeValue == request.AttributeValue);

        var attribute = new Attribute
        (
            request,
            id: attributeDb?.Id
        );

        try
        {
            if (attributeDb is null)
                await _repository.AddAsync(attribute);
            else
                _repository.Update(attribute);

            await Commit();

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return (Response.Valid(), attribute);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);

            return (Response.Invalid("Attribute", exc.Message), attribute);
        }
    }

    public async Task<Response> RemoveAttributeByProduct(RemoveAttributeRequest request, CancellationToken cancellationToken = default)
    {
        var attribute = await _repository.FindByAsync(x => x.ProductId == request.ProductId);

        if (attribute is null)
            return Response.Invalid("Attribute", "Attribute not found");

        _repository.Remove(attribute);

        await Commit();

        return Response.Valid();
    }
}