using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Services;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using FluentAssertions;
using Moq;
using System.Collections.Generic;

namespace Acropolis.Tests.Application.Features.Promotions.Services;

public class PromotionCardAppServiceTests
{

    [Fact(DisplayName = "Should return product without discount when get a product without sellerId")]
    public async Task GetProductCardAsync_PromotionWithoutSellerId_ShouldReturnProductWithoutDiscountResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, productRepositoryMock, customerRepositoryMock, service) = PromotionProductCardContextMock();
        var sellerId = Guid.Empty;
        var productId = Guid.NewGuid();
        var cnpj = "97194593000106";

        //Act
        var command = new ProductCardRequest(cnpj)
        {
            ProductsList = new List<ProductSellerRequest>() {
            new ProductSellerRequest() {
                Id = productId.ToString(), SellerId = sellerId
    }
        }
        };
        var response = await service.ProductListHasPromotionAsync(command);

        //Assert
        response.Products.Should().HaveCount(1);
        response.Products.Should().AllSatisfy(product =>
        {
            product.PromotionMessage.Should().BeEmpty();
            product.HasDiscount.Should().BeFalse();
        });
    }


    [Fact(DisplayName = "Should return product without discount when get a product with promotion without skus")]
    public async Task GetProductCardAsync_PromotionWithoutSkus_ShouldReturnProductWithoutDiscountResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, productRepositoryMock, customerRepositoryMock, service) = PromotionProductCardContextMock();
        var sellerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cnpj = "97194593000106";
        var promotionExternalId = "p-ext111";

        var promotionsCnpjs = new List<PromotionCnpj>()
        {
            new PromotionCnpj(promotionExternalId, cnpj, sellerId)
        };

        promotionCnpjRepositoryMock.Setup(promotionCnpj => promotionCnpj.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Guid>> ())).Returns(Task.FromResult(promotionsCnpjs));
        promotionRepositoryMock.Setup(promotion => promotion.FindSkusByAsync(It.IsAny<List<Guid>>())).Returns(Task.FromResult(new List<string>()));

        //Act
        var command = new ProductCardRequest(cnpj)
        {
            ProductsList = new List<ProductSellerRequest>() {
            new ProductSellerRequest() {
                Id = productId.ToString(), SellerId = Guid.NewGuid()
    }
        }
        };
        var response = await service.ProductListHasPromotionAsync(command);

        //Assert
        response.Products.Should().HaveCount(1);
        response.Products.Should().AllSatisfy(product => {
            product.PromotionMessage.Should().BeEmpty();
            product.HasDiscount.Should().BeFalse();
        });
    }
        
    private static (Mock<IPromotionRepository> promotionRepositoryMock, Mock<IPromotionCnpjRepository> promotionCnpjRepositoryMock, Mock<IProductRepository> productRepositoryMock, Mock<ICustomerRepository> customerRepositoryMock, IProductCardAppService service) PromotionProductCardContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var customerRepositoryMock = new Mock<ICustomerRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        
        var service = new ProductCardAppService(unitOfWorkMock.Object, promotionRepositoryMock.Object, promotionCnpjRepositoryMock.Object, productRepositoryMock.Object, customerRepositoryMock.Object);

        return (promotionRepositoryMock, promotionCnpjRepositoryMock, productRepositoryMock, customerRepositoryMock, service);
    }
}