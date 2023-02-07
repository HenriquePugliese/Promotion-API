using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Products.Validators;
using Acropolis.Consumer.Features.Product.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Services;

public class ProductCreatedService : IConsumerService<ProductCreatedMessage>
{
    private readonly IProductAppService _productAppService;
    private readonly ILogger<ProductCreatedService> _logger;
    private readonly IMapper _mapper;

    public ProductCreatedService(IProductAppService productAppService, ILogger<ProductCreatedService> logger, IMapper mapper)
    {
        _productAppService = productAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(ProductCreatedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(ProductCreatedMessage));

        var validateProduct = new CreateProductValidation();
        var resultValidation = await validateProduct.ValidateAsync(message);

        if (resultValidation.IsValid)
        {
            var (response, createdProduct) = await _productAppService.CreateProduct(_mapper.Map<CreateProductRequest>(message));

            if (!response.IsValid() || createdProduct is null)
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(d => d.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(ProductCreatedMessage),
            JsonConvert.SerializeObject(resultValidation
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));
    }
}