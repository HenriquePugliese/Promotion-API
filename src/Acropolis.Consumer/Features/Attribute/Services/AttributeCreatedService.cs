using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Attributes.Validators;
using Acropolis.Consumer.Features.Attribute.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Attribute.Services;

public class AttributeCreatedService : IConsumerService<AttributeCreatedMessage>
{
    private readonly IAttributeAppService _attributeAppService;
    private readonly ILogger<AttributeCreatedService> _logger;
    private readonly IMapper _mapper;

    public AttributeCreatedService(
        IAttributeAppService attributeAppService,
        ILogger<AttributeCreatedService> logger,
        IMapper mapper)
    {
        _attributeAppService = attributeAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(AttributeCreatedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(AttributeCreatedMessage));

        var validator = new CreateAttributeValidation();
        var validatorResult = await validator.ValidateAsync(message);

        if (validatorResult.IsValid)
        {
            var (response, createdAtrribute) = await _attributeAppService.CreateAttributeConsumer(_mapper.Map<CreateAttributeRequest>(message));

            if (!response.IsValid() || createdAtrribute is null)
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(d => d.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(AttributeCreatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()).ToString());
    }
}