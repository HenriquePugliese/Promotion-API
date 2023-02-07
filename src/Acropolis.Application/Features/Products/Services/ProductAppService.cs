using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Acropolis.Application.Features.Products.Services;

public class ProductAppService : AppService, IProductAppService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductAppService> _logger;

    public ProductAppService(IUnitOfWork unitOfWork, IProductRepository repository, ILogger<ProductAppService> logger)
        : base(unitOfWork)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<(Response, Product?)> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Create new product({request.Id})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        var product = new Product(request);

        try
        {
            var productRepository = await _repository.GetByIdAsync(request.Id);

            if (productRepository != null)
                return (Response.Invalid("Product Create", "Product already exists and cannot be created."), null);

            await _repository.AddAsync(product);
            await Commit();

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return (Response.Valid(), product);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);
            return (Response.Invalid("Product Create", exc.Message), null);
        }
    }

    public async Task<(Response, Product?)> UpdateProduct(UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Update product ({request.Id})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        try
        {
            var productRepository = await _repository.GetByIdAsync(request.Id);

            if (productRepository is null)
            {
                var createProductRequest = new CreateProductRequest()
                {
                    Id = request.Id,
                    MaterialCode = request.MaterialCode,
                    Name = request.Name,
                    SellerId = request.SellerId,
                    Status = request.Status,
                    UnitMeasure = request.UnitMeasure,
                    UnitWeight = request.UnitWeight,
                    Weight = request.Weight
                };

                return await CreateProduct(createProductRequest, cancellationToken);
            }

            productRepository.Update(request);
            _repository.Update(productRepository);
            await Commit();

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return (Response.Valid(), productRepository);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);
            return (Response.Invalid("Product Update", exc.Message), null);
        }
    }

    public async Task<Response> RemoveProduct(RemoveProductRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Remove product: ({request.Id})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        try
        {
            var product = await _repository.GetByIdAsync(request.Id);

            if (product is null)
                return Response.Invalid("Product Remove", "Product not found");

            _repository.Remove(product);
            await Commit();

            return Response.Valid();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);
            return Response.Invalid("Product Remove", exc.Message);
        }
    }
}
