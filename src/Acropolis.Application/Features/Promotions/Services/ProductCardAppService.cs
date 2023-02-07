using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using System.Linq;

namespace Acropolis.Application.Features.Promotions.Services;
public class ProductCardAppService : AppService, IProductCardAppService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionCnpjRepository _promotionCnpjRepository;
    private readonly IProductRepository _productRepository;
    
    public ProductCardAppService(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IPromotionCnpjRepository promotionCnpjRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        : base(unitOfWork, promotionRepository, promotionCnpjRepository, customerRepository)
    {
        _promotionRepository = promotionRepository;
        _promotionCnpjRepository = promotionCnpjRepository;
        _productRepository = productRepository;
    }

    public async Task<ProductCardResponse> ProductListHasPromotionAsync(ProductCardRequest productCardRequest)
    {
        var sellersIds = productCardRequest.ProductsList.Select(p => p.SellerId).Distinct();
        
        if (sellersIds.Contains(Guid.Empty))
            return BuildFailiureResponse(productCardRequest.CNPJ, productCardRequest.ProductsList);

        var promotionsCnpjs = await _promotionCnpjRepository.GetPromotionCNPJByCNPJAsync(productCardRequest.CNPJ, sellersIds);

        if (!promotionsCnpjs.Any())
            return BuildFailiureResponse(productCardRequest.CNPJ, productCardRequest.ProductsList);        

        var (isValidPromotionsParameters, _) = await ValidatePromotionsParameters(promotionsCnpjs, productCardRequest.CNPJ);

        if (!isValidPromotionsParameters)
            return BuildFailiureResponse(productCardRequest.CNPJ, productCardRequest.ProductsList);

        var products = await _productRepository.FindProductIdsAsync(productCardRequest.ProductsList.Select(p => new Guid(p.Id)).Distinct().ToList(), promotionsCnpjs.Select(p => p.SellerId).ToList());

        var productCardResponse = new ProductCardResponse(productCardRequest.CNPJ);

        productCardRequest.ProductsList.ForEach(i =>
            productCardResponse.Products.Add(new ProductPromotionResponse(i.Id, products.Count > 0 && products.Contains(Guid.Parse(i.Id)))));

        return productCardResponse;
    }

    private static ProductCardResponse BuildFailiureResponse(string cnpj, List<ProductSellerRequest> products)
    {
        var productCardResponse = new ProductCardResponse(cnpj);
        
        products.ForEach(p => productCardResponse.Products.Add(new ProductPromotionResponse(p.Id, false)));
        
        return productCardResponse;
    } 
        
}
