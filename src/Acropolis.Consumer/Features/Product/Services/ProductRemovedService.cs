using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Products.Validators;
using Acropolis.Consumer.Features.Product.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Services;

public class ProductRemovedService : IConsumerService<ProductRemovedMessage>
{
    private readonly IProductAppService _productAppService;
    private readonly ILogger<ProductRemovedService> _logger;
    private readonly IMapper _mapper;

    public ProductRemovedService(IProductAppService productAppService, ILogger<ProductRemovedService> logger, IMapper mapper)
    {
        _productAppService = productAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(ProductRemovedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(ProductRemovedMessage));

        var validateProduct = new RemoveProductValidation();
        var resultValidation = await validateProduct.ValidateAsync(message);

        if (resultValidation.IsValid)
        {
            var response = await _productAppService.RemoveProduct(_mapper.Map<RemoveProductRequest>(message));

            if (!response.IsValid())
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(d => d.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(ProductRemovedMessage),
            JsonConvert.SerializeObject(resultValidation
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));
    }
}