using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Attributes.Requests;
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
using FluentAssertions;
using Moq;
using Nest;
using System.Linq.Expressions;
using Attribute = Acropolis.Application.Features.Attributes.Attribute;
using Customer = Acropolis.Application.Features.Customers.Customer;

namespace Acropolis.Tests.Application.Features.Promotions.Services;

public class PromotionOrderAppServiceTests
{
    [Fact(DisplayName = "Should return invalid response when customer not exist")]
    public async Task GetPromotionOrderAsync_InvalidCustomerCode_ShouldReturnInvalidResponse()
    {
        //Arrange
        var sellerId = Guid.NewGuid();
        var (_, _, _, _, _, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var command = new PromotionOrderRequest()
        {
            CustomerCode = "971-NotExists",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = Guid.NewGuid()
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, objectResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.Contains("Cliente"));
        objectResponse.HasDiscount.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response promotion not found when promotion not exist to customer")]
    public async Task GetPromotionOrderAsync_ValidCustomerCode_ShouldReturnInvalidResponsePromotionNotFound()
    {
        //Arrange
        var sellerId = Guid.NewGuid();
        var (_, _, _, _, _, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var command = new PromotionOrderRequest()
        {
            CustomerCode = "971-NotExists",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = Guid.NewGuid()
                }
            },
            SellerId = sellerId
        };

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>()
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));

        //Act
        var (response, objectResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.Contains("CustomerCode"));
        objectResponse.HasDiscount.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when user parameters not match with promotion parameters")]
    public async Task GetPromotionOrderAsync_ValidCustomerCode_ShouldReturnInvalidResponseUserParametersNotMatchPromotionParameters()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = new Product(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "0001",
            Name = "Produto 1",
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var products = new List<Product>(){
            product
        };

        var attribute = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" } } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "RJ" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>
        {
            promotion
        };

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, "97194593000106", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>> ())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = productId
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, objectResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.Contains("cadastrados"));
        objectResponse.HasDiscount.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when promotion by sellerId, productItems, cnpj not found")]
    public async Task GetPromotionOrderAsync_ValidOrderRequest_ShouldReturnInvalidResponsePromotionNotFound()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = new Product(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "0001",
            Name = "Produto 1",
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var products = new List<Product>(){
            product
        };

        var attribute = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" } } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>();
        listPromotions.Add(promotion);

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, "97194593000106", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(new List<PromotionResponse>()));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = productId
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, objectResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.Contains("encontradas"));
        objectResponse.HasDiscount.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when get promotion order with product without promotion")]
    public async Task GetPromotionOrderAsync_ProductWithoutPromotion_ShouldReturnInvalidResponse()
    {
        //Arrange
        var (_, _, _, productRepositoryMock, _, _, service) = PromotionDiscountContextMock();
        var command = new PromotionOrderRequest()
        {
            CustomerCode = "971-cs",
            ProductItems = new List<ProductAmountRequest>(){ 
                new ProductAmountRequest(){ 
                    Amount = 2, 
                    ProductId = Guid.NewGuid() 
                } 
            },
            SellerId = Guid.NewGuid()
        };

        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult(Enumerable.Empty<Product>()));

        //Act
        var (response, _) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();
    }

    [Fact(DisplayName = "Should return order bar with zero percent response when get promotion order with zero amount")]
    public async Task GetPromotionOrderAsync_ProductWithZeroAmount_ShouldReturnOrderBarWithZeroPercentResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = new Product(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "0001",
            Name = "Produto 1",
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var products = new List<Product>(){
            product
        };

        var attribute = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" } } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>();
        listPromotions.Add(promotion);

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 0,
                    ProductId = productId
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.OrderBar?.Percentage.Should().Be(decimal.Zero);
    }

    [Fact(DisplayName = "Should return invalid response when get promotion order with non-existent product")]
    public async Task GetPromotionOrderAsync_NonExistentProduct_ShouldReturnInvalidResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = new Product(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "000122222",
            Name = "Produto 1NE",
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var products = new List<Product>(){
            product
        };

        var attribute = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" } } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>();
        listPromotions.Add(promotion);

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult(Enumerable.Empty<Product>()));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 0,
                    ProductId = productId
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeFalse();        
    }

    [Fact(DisplayName = "Should return valid response when user get discount limit")]
    public async Task GetPromotionOrderAsync_ExistingDiscount_ShouldReturnValidResponseWhenGetDiscountLimit()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null),
                new Acropolis.Application.Features.Parameters.Parameter("Campinas", "MesoRegiao", "110", true, new Guid("8E389E1D-0D3B-45F9-A7D7-066A9D35168E"), null),
                new Acropolis.Application.Features.Parameters.Parameter("Lapa", "MicroRegiao", "975", true, new Guid("9E389E1D-0D3B-45F9-A7D7-066A9D35169E"), null),
                new Acropolis.Application.Features.Parameters.Parameter("Campinas", "Segmento", "110", true, new Guid("1E389E1D-0D3B-45F9-A7D7-066A9D35161E"), null)
            }
        };

        var product1 = new Product(new CreateProductRequest()
        {
            Id = Guid.NewGuid(),
            SellerId = sellerId,
            MaterialCode = "0001",
            Name = "Produto 1",
            UnitMeasure = "to",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var product2 = new Product(new CreateProductRequest()
        {
            Id = Guid.NewGuid(),
            SellerId = sellerId,
            MaterialCode = "0002",
            Name = "Produto 2",
            UnitMeasure = "kg",
            UnitWeight = "1",
            Weight = 1,
            Status = 1
        });

        var products = new List<Product>(){
            product1,
            product2
        };

        var attribute1 = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product1.Id
        });

        var attribute2 = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product2.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute1,
            attribute2
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute1.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product1.MaterialCode}", $"{product2.MaterialCode}" } }},
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { 
                UF = new List<string>() { "SP", "RJ" },
                MesoRegiao = new List<string>() { "102", "110", "350"},
                MicroRegiao = new List<string>() { "930", "945", "975"}
            } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>();
        listPromotions.Add(promotion);

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, "97194593000106", sellerId, promotion)
        };

        var promotionCnpjDiscountLimit = new List<PromotionCnpjDiscountLimit>()
        {
            new PromotionCnpjDiscountLimit("97194593000106", 20, sellerId)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(promotionCnpjDiscountLimit));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = product1.Id,
                },
                new ProductAmountRequest(){
                    Amount = 1,
                    ProductId = product2.Id,
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.HasDiscount.Should().BeTrue();
        promotionOrderResponse?.OrderBar?.MaxDiscountPercentage.Should().BeGreaterThan(decimal.Zero);
        promotionOrderResponse?.OrderBar?.Percentage.Should().Be(20);
    }

    [Fact(DisplayName = "Should return valid response when get promotion order")]
    public async Task GetPromotionOrderAsync_ExistingDiscount_ShouldReturnValidResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = new Product(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "0001",
            Name = "Produto 1",
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10,
            Status = 1
        });

        var products = new List<Product>(){
            product
        };

        var attribute = new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = new Guid("c7fbc3fd-34d6-4573-a089-aa5ce26B65b5"),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = product.Id
        });

        var attributes = new List<Attribute>()
        {
            attribute
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" } } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 30, TotalAttributes = 1, GreaterEqualValue = 1 } },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>();
        listPromotions.Add(promotion);

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>(){ promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = productId
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.HasDiscount.Should().BeTrue();
        promotionOrderResponse?.OrderBar?.MaxDiscountPercentage.Should().BeGreaterThan(decimal.Zero);
        promotionOrderResponse?.OrderBar?.Percentage.Should().Be(30);
    }

    [Fact(DisplayName = "Should return success message when current discount greater than maximum discount")]
    public async Task GetPromotionOrderAsync_ReachedMaxDiscount_ShouldReturnValidResponseSuccessMessage()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = SetProduct(1, sellerId);
        var product2 = SetProduct(2, sellerId);
        var product3 = SetProduct(3, sellerId);

        var products = new List<Product>(){
            product, product2, product3
        };

        var attribute = SetAttribute(product.Id);
        var attribute2 = SetAttribute(product2.Id);
        var attribute3 = SetAttribute(product3.Id);

        var attributes = new List<Attribute>()
        {
            attribute, attribute2, attribute3
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { 
                new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute2.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product2.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute3.AttributeValueId.ToString(), Qtd = 1, SKUs = new List<string>() { $"{product3.MaterialCode}" }}
            },
            Rules = new List<CreatePromotionRuleRequest>() { 
                new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 12, TotalAttributes = 1, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 10, TotalAttributes = 1, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 2, TotalAttributes = 2, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 6, TotalAttributes = 2, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 11, TotalAttributes = 2, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 7, TotalAttributes = 3, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 12, TotalAttributes = 3, GreaterEqualValue = 30 }
            },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>
        {
            promotion
        };

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = product.Id
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.HasDiscount.Should().BeTrue();
        promotionOrderResponse?.IncentiveMessge.Should().Contain("conseguiu");
        promotionOrderResponse?.IncentiveAttributes.Count.Should().Be(0);
    }

    [Fact(DisplayName = "Should return success message when cross and up sell incentive flow")]
    public async Task GetPromotionOrderAsync_CrossUpSellIncentive_ShouldReturnValidResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = SetProduct(1, sellerId);
        var product2 = SetProduct(2, sellerId);
        var product3 = SetProduct(3, sellerId);

        var products = new List<Product>(){
            product, product2, product3
        };

        var attribute = SetAttribute(product.Id);
        var attribute2 = SetAttribute(product2.Id);
        var attribute3 = SetAttribute(product3.Id);

        var attributes = new List<Attribute>()
        {
            attribute, attribute2, attribute3
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() {
                new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute2.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product2.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute3.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product3.MaterialCode}" }}
            },
            Rules = new List<CreatePromotionRuleRequest>() {
                new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 5, TotalAttributes = 1, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 10, TotalAttributes = 1, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 2, TotalAttributes = 2, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 6, TotalAttributes = 2, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 11, TotalAttributes = 2, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 7, TotalAttributes = 3, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 12, TotalAttributes = 3, GreaterEqualValue = 30 }
            },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>
        {
            promotion
        };

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = product.Id
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.HasDiscount.Should().BeTrue();
        promotionOrderResponse?.IncentiveMessge.Should().Contain("quantidade");
        promotionOrderResponse?.IncentiveAttributes.Count.Should().Be(3);
    }

    [Fact(DisplayName = "Should return success message when cross sell incentive flow")]
    public async Task GetPromotionOrderAsync_CrossSellIncentive_ShouldReturnValidResponse()
    {
        //Arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, service) = PromotionDiscountContextMock();
        var sellerId = Guid.NewGuid();
        var externalId = "p-ext01";

        var customer = new Customer("97194593000106", sellerId.ToString(), "971-cs")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };

        var product = SetProduct(1, sellerId);
        var product2 = SetProduct(2, sellerId);
        var product3 = SetProduct(3, sellerId);

        var products = new List<Product>(){
            product, product2, product3
        };

        var attribute = SetAttribute(product.Id);
        var attribute2 = SetAttribute(product2.Id);
        var attribute3 = SetAttribute(product3.Id);

        var attributes = new List<Attribute>()
        {
            attribute, attribute2, attribute3
        };

        var promotion = new Promotion(new CreatePromotionRequest()
        {
            Name = "Promotion valid",
            ExternalId = externalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() {
                new CreatePromotionAttributeRequest() { AttributesId = attribute.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute2.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product2.MaterialCode}" }},
                new CreatePromotionAttributeRequest() { AttributesId = attribute3.AttributeValueId.ToString(), Qtd = 0.01m, SKUs = new List<string>() { $"{product3.MaterialCode}" }}
            },
            Rules = new List<CreatePromotionRuleRequest>() {
                new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 5, TotalAttributes = 1, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 10, TotalAttributes = 1, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 2, TotalAttributes = 2, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 6, TotalAttributes = 2, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 11, TotalAttributes = 2, GreaterEqualValue = 30 },
                new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 1 },
                new CreatePromotionRuleRequest() { Percentage = 7, TotalAttributes = 3, GreaterEqualValue = 20 },
                new CreatePromotionRuleRequest() { Percentage = 12, TotalAttributes = 3, GreaterEqualValue = 30 }
            },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, sellerId);

        var promotions = new List<PromotionResponse>()
        {
            new PromotionResponse(promotion)
        };

        var listPromotions = new List<Promotion>
        {
            promotion
        };

        var promotionCnpjs = new List<PromotionCnpj>(){
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        var promotionCNPJByCNPJ = new List<PromotionCnpj>()
        {
            new PromotionCnpj(externalId, $"{customer.Cnpj}", sellerId, promotion)
        };

        customerRepositoryMock.Setup(repository => repository.GetByCustomerCodeAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Customer?>(customer));
        customerRepositoryMock.Setup(repository => repository.FindBy(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(Task.FromResult<Customer?>(customer));
        productRepositoryMock.Setup(repository => repository.FindAllByIdsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.FromResult<IEnumerable<Product>>(products));
        attributeRepositoryMock.Setup(repository => repository.FindAllByAsync(It.IsAny<Expression<Func<Attribute, bool>>>())).Returns(Task.FromResult<IEnumerable<Attribute>>(attributes));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionsByCNPJAsync(It.IsAny<string>())).Returns(Task.FromResult(promotionCnpjs));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(promotionCnpjs));
        promotionRepositoryMock.Setup(repository => repository.FindBySKUsAsync(It.IsAny<PromotionProductParameters>())).Returns(Task.FromResult<IEnumerable<PromotionResponse>>(promotions));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult(new List<Promotion>() { promotion }));
        promotionCnpjDiscountLimitRepositoryMock.Setup(repository => repository.GetObjectsCnpjsWithDiscountLimitsAsync(It.IsAny<List<string>>(), sellerId)).Returns(Task.FromResult<IEnumerable<PromotionCnpjDiscountLimit>>(Enumerable.Empty<PromotionCnpjDiscountLimit>()));
        promotionCnpjRepositoryMock.Setup(repository => repository.GetPromotionCNPJByCNPJAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<List<PromotionCnpj>>(promotionCNPJByCNPJ));
        promotionRepositoryMock.Setup(repository => repository.GetByAsync(It.IsAny<List<PromotionCnpj>>())).Returns(Task.FromResult<List<Promotion>>(listPromotions));

        var command = new PromotionOrderRequest()
        {
            CustomerCode = $"{customer?.CustomerCode}",
            ProductItems = new List<ProductAmountRequest>(){
                new ProductAmountRequest(){
                    Amount = 2,
                    ProductId = product.Id
                },
                new ProductAmountRequest(){
                    Amount = 3,
                    ProductId = product2.Id
                }
            },
            SellerId = sellerId
        };

        //Act
        var (response, promotionOrderResponse) = await service.GetPromotionOrderAsync(command);

        //Assert
        response.IsValid().Should().BeTrue();
        response.Notifications.Should().BeEmpty();

        promotionOrderResponse?.HasDiscount.Should().BeTrue();
        promotionOrderResponse?.IncentiveMessge.Should().Contain("garantir");
        promotionOrderResponse?.IncentiveAttributes.Count.Should().Be(1);
    }

    private static (Mock<IPromotionRepository> promotionRepositoryMock,
        Mock<IPromotionCnpjRepository> promotionCnpjRepositoryMock,
        Mock<IPromotionCnpjDiscountLimitRepository> promotionCnpjDiscountLimitRepositoryMock,
        Mock<IProductRepository> productRepositoryMock,
        Mock<IAttributeRepository> attributeRepositoryMock,
        Mock<ICustomerRepository> customerRepositoryMock,
        IPromotionDiscountAppService Service) PromotionDiscountContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();
        var promotionCnpjDiscountLimitRepositoryMock = new Mock<IPromotionCnpjDiscountLimitRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var attributeRepositoryMock = new Mock<IAttributeRepository>();
        var customerRepositoryMock = new Mock<ICustomerRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        var promotionDiscountService = new PromotionOrderAppService(unitOfWorkMock.Object, promotionRepositoryMock.Object, promotionCnpjRepositoryMock.Object, promotionCnpjDiscountLimitRepositoryMock.Object, productRepositoryMock.Object, attributeRepositoryMock.Object, customerRepositoryMock.Object);

        return (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionCnpjDiscountLimitRepositoryMock, productRepositoryMock, attributeRepositoryMock, customerRepositoryMock, promotionDiscountService);
    }

    private static Product SetProduct(int codProduct, Guid sellerId)
    {
        return new Product(new CreateProductRequest()
        {
            Id = Guid.NewGuid(),
            SellerId = sellerId,
            MaterialCode = "000" + codProduct,
            Name = "Produto " + codProduct,
            UnitMeasure = "kg",
            UnitWeight = codProduct + "0",
            Weight = 10 * codProduct,
            Status = 1
        });
    }

    private static Attribute SetAttribute(Guid idProduct)
    {
        return new Attribute(new CreateAttributeRequest()
        {
            AttributeKeyId = Guid.NewGuid(),
            AttributeKey = string.Empty,
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = true,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = string.Empty,
            AttributeValueId = Guid.NewGuid(),
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            ProductId = idProduct
        });
    }
}