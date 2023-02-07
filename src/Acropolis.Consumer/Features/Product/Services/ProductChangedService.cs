using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Products.Validators;
using Acropolis.Consumer.Features.Product.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Services;

public class ProductChangedService : IConsumerService<ProductChangedMessage>
{
    private readonly IProductAppService _productAppService;
    private readonly ILogger<ProductChangedService> _logger;
    private readonly IMapper _mapper;

    public ProductChangedService(IProductAppService productAppService, ILogger<ProductChangedService> logger, IMapper mapper)
    {
        _productAppService = productAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(ProductChangedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(ProductChangedMessage));

        var validateProduct = new UpdateProductValidation();
        var resultValidation = await validateProduct.ValidateAsync(message);

        if (resultValidation.IsValid)
        {
            var (response, updatedProduct) = await _productAppService.UpdateProduct(_mapper.Map<UpdateProductRequest>(message));

            if (!response.IsValid() || updatedProduct is null)
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(d => d.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(ProductChangedMessage),
            JsonConvert.SerializeObject(resultValidation
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));
    }
}