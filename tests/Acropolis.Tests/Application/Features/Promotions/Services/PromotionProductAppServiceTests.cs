using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Customers;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Infrastructure.Repositories;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Acropolis.Tests.Application.Features.Promotions.Services;

public class PromotionProductAppServiceTests
{
    [Fact(DisplayName = "Should return invalid response when get a product not found")]
    public async Task GetProductIncentiveListAsync_GetProductNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var (promotionRepositoryMock, productRepositoryMock, _, _, _, service) = PromotionProductContextMock();
        var sellerId = Guid.NewGuid().ToString();
        var productId = Guid.NewGuid().ToString();
        var cnpj = "97194593000106";

        productRepositoryMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).Returns(Task.FromResult<Product?>(null));

        //Act
        var command = new GetPromotionProductIncentiveListRequest(productId, sellerId, cnpj);
        var (response, _) = await service.GetProductIncentiveListAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.ToLower().Contains("produto"));
    }

    [Fact(DisplayName = "Should return invalid response when get a product incentive list with product id invalid")]
    public async Task GetProductIncentiveListAsync_GetProductWithProductIdInvalid_ShouldReturnInvalidResponse()
    {
        //Arrange
        var (_, _, _, _, _, service) = PromotionProductContextMock();
        var sellerId = Guid.NewGuid().ToString();
        var productId = "product-invalid-id";
        var cnpj = "97194593000106";
        var command = new GetPromotionProductIncentiveListRequest(productId, sellerId, cnpj);

        //Act
        var (response, _) = await service.GetProductIncentiveListAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.ToLower().Contains("identificador produto"));
    }

    [Fact(DisplayName = "Should return invalid response when get a product incentive list with seller id invalid")]
    public async Task GetProductIncentiveListAsync_GetProductWithSellerIdInvalid_ShouldReturnInvalidResponse()
    {
        //Arrange
        var (_, _, _, _, _, service) = PromotionProductContextMock();
        var sellerId = "seller-invalid-id";
        var productId = Guid.NewGuid().ToString();
        var cnpj = "97194593000106";
        var command = new GetPromotionProductIncentiveListRequest(productId, sellerId, cnpj);

        //Act
        var (response, _) = await service.GetProductIncentiveListAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.ToLower().Contains("identificador seller"));
    }

    [Theory(DisplayName = "Should return valid response when get a product incentive list")]
    [MemberData(nameof(Data))]
    public async Task GetProductIncentiveListAsync_GetExistingProduct_ShouldReturnValidResponse(CreateProductRequest createProduct)
    {
        //Arrange
        var (promotionRepositoryMock, productRepositoryMock, attributeRepositoryMock, promotionCnpjRepositoryMock, customerRepositoryMock, service) = PromotionProductContextMock();
        var sellerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cnpj = "97194593000106";

        createProduct.Id = productId;
        createProduct.SellerId = sellerId;

        var product = new Product(createProduct);

        var productsIds = new List<Guid>(){
            productId,
            Guid.NewGuid()
        };

        var attributesIds = new List<Guid>(){
            new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5")
        };

        var promotionsCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj("ext-adsd", "68845801000191", Guid.NewGuid())
        };

        var customer = new Customer(cnpj, sellerId.ToString(), "cc-1asds")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("Santa Catarina", "UF", "SC", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null),
                new Acropolis.Application.Features.Parameters.Parameter("Campinas", "MesoRegiao", "110", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var promotion = PromotionHelper.CreateValidPromotion(sellerId);
        
        productRepositoryMock.Setup(repository => repository.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>())).Returns(Task.FromResult<Product?>(product));
        productRepositoryMock.Setup(repository => repository.FindAllIdsByAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<int>())).Returns(Task.FromResult<IEnumerable<Guid>>(productsIds));

        attributeRepositoryMock.Setup(repository => repository.GetAllAttributesValuesIdsByProductIdAsync(productId)).Returns(Task.FromResult<IEnumerable<Guid>>(attributesIds));
        attributeRepositoryMock.Setup(repository => repository.FindProductsIdsAsync(It.IsAny<AttributeParameters>())).Returns(Task.FromResult<IEnumerable<Guid>>(productsIds));

        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionsCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindWithProductAttributesAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(new List<PromotionResponse>() { new PromotionResponse(promotion) }));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));

        //Act
        var command = new GetPromotionProductIncentiveListRequest(productId.ToString(), sellerId.ToString(), cnpj);
        var (response, productIncentiveListResponse) = await service.GetProductIncentiveListAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        productIncentiveListResponse?.ProductId.Should().Be(productId.ToString());
        productIncentiveListResponse?.SellerId.Should().Be(sellerId.ToString());
        productIncentiveListResponse?.IncentiveList.Should().NotBeEmpty();
        productIncentiveListResponse?.HasDiscount.Should().BeTrue();
        productIncentiveListResponse?.MaterialCode.Should().BeGreaterThanOrEqualTo(0);
    }

    private static (Mock<IPromotionRepository> promotionRepositoryMock, Mock<IProductRepository> productRepositoryMock, Mock<IAttributeRepository> attributeRepositoryMock, Mock<IPromotionCnpjRepository> promotionCnpjRepositoryMock, Mock<ICustomerRepository> customerRepositoryMock, IPromotionProductAppService Service) PromotionProductContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var attributeRepositoryMock = new Mock<IAttributeRepository>();
        var customerRepositoryMock = new Mock<ICustomerRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        var promotionProductService = new PromotionProductAppService(unitOfWorkMock.Object, promotionRepositoryMock.Object, productRepositoryMock.Object, attributeRepositoryMock.Object, promotionCnpjRepositoryMock.Object, customerRepositoryMock.Object);

        return (promotionRepositoryMock, productRepositoryMock, attributeRepositoryMock, promotionCnpjRepositoryMock, customerRepositoryMock, promotionProductService);
    }

    public static IEnumerable<object[]> Data =>
         new List<object[]>
         {
            new object[] {
                new CreateProductRequest(){
                    MaterialCode = "0001",
                    Name = "Produto 1",
                    UnitMeasure = "kg",
                    UnitWeight = "10",
                    Weight = 10,
                    Status = 1
                }
            },
            new object[] {
                new CreateProductRequest(){
                    MaterialCode = null,
                    Name = "Produto 1",
                    UnitMeasure = "kg",
                    UnitWeight = "10",
                    Weight = 10,
                    Status = 1
                }
            }
         };
}