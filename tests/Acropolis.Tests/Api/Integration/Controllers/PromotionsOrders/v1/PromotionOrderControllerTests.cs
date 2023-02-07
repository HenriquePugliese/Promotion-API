using Acropolis.Api;
using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace Acropolis.Tests.Api.Integration.Controllers.PromotionsOrders.v1;

public class PromotionOrderControllerTests : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;

    public PromotionOrderControllerTests()
    {
        _client = GetTestAppClient();
    }

    [Theory(DisplayName = "Should return valid response when get a promotion order")]
    [MemberData(nameof(AttributesCrossSellData))]
    [MemberData(nameof(AttributesUpSellData))]
    public async Task GetPromotionOrder_GetExistingPromotionOrder_ShouldReturnValidResponse(Guid[] attributesIds)
    {
        //Arrange
        var command = await GetValidPromotionOrderRequest();

        await CreatePromotionOrder(command, attributesIds);

        //Act
        var response = await _client.PostAsJsonAsync($"/v1/desconto-mais/promotion-order", command);
      
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Should return invalid response when get promotion order with empty product items")]
    public async Task GetPromotionOrder_EmptyProductItems_ShouldReturnInvalidResponse()
    {
        //Arrange
        var command = await GetValidPromotionOrderRequest();
        
        command.ProductItems = Enumerable.Empty<ProductAmountRequest>();

        //Act
        var response = await _client.PostAsJsonAsync($"/v1/desconto-mais/promotion-order", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return invalid response when get promotion order with empty sellerid")]
    public async Task GetPromotionOrder_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        var command = await GetValidPromotionOrderRequest();

        command.SellerId = Guid.Empty;

        //Act
        var response = await _client.PostAsJsonAsync($"/v1/desconto-mais/promotion-order", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);        
    }

    [Fact(DisplayName = "Should return invalid response when get promotion order with empty customer code")]
    public async Task GetPromotionOrderAsync_EmptyCustomerCode_ShouldReturnInvalidResponse()
    {
        //Arrange
        var command = await GetValidPromotionOrderRequest();

        command.CustomerCode = string.Empty;

        //Act
        var response = await _client.PostAsJsonAsync($"/v1/desconto-mais/promotion-order", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static IEnumerable<object[]> AttributesCrossSellData()
    {
        yield return new object[] { new Guid[]{
            new Guid("766679ea-8b45-41fa-a2a5-294aad64b2dd"),
            new Guid("2C7A038F-19FC-4B99-94BB-E79DCDCE1F11")
        }};        
    }

    public static IEnumerable<object[]> AttributesUpSellData()
    {
        yield return new object[] { new Guid[]{
            new Guid("766679ea-8b45-41fa-a2a5-294aad64b2dd")
        }};
    }

    private async Task<PromotionOrderRequest> GetValidPromotionOrderRequest()
    {
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var customerCode = "acb-cc-1";
        var productsIds = await GetValidProductsIds(sellerId);

        return new PromotionOrderRequest()
        {
            CustomerCode = customerCode,
            SellerId = sellerId,
            ProductItems = new List<ProductAmountRequest>()
            {
                new ProductAmountRequest()
                {
                    Amount = 2,
                    ProductId = productsIds.First(),
                },
                new ProductAmountRequest()
                {
                    Amount = 2,
                    ProductId = productsIds.Last(),
                }
            }
        };
    }

    private async Task<IEnumerable<Guid>> GetValidProductsIds(Guid sellerId){
        using var scope = ServiceProvider?.CreateScope();

        if (scope is null)
            return Enumerable.Empty<Guid>();

        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var productIds = await productRepository.FindAllIdsByAsync(product => product.MaterialCode != null && product.MaterialCode.Contains("mtc-") && product.SellerId == sellerId, 2);
        
        return productIds;
    }

    private async Task CreatePromotionOrder(PromotionOrderRequest promotionOrderRequest, Guid[] attributesIds)
    {
        using var scope = ServiceProvider?.CreateScope();

        if (scope is null)
            return;

        var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var productAppService = scope.ServiceProvider.GetRequiredService<IProductAppService>();
        var attributeAppService = scope.ServiceProvider.GetRequiredService<IAttributeAppService>();
        var promotionAppService = scope.ServiceProvider.GetRequiredService<IPromotionAppService>();

        var promotionExternalId = Guid.NewGuid().ToString();
        var customer = await customerRepository.GetByCustomerCodeAsync(promotionOrderRequest.CustomerCode, promotionOrderRequest.SellerId);
        var promotionAttributesRequest = new List<CreatePromotionAttributeRequest>();

        for(var index=0; index < attributesIds.Length; index++)
        {
            promotionAttributesRequest.Add(new CreatePromotionAttributeRequest()
            {
                AttributesId = attributesIds[index].ToString(),
                Qtd = 1,
                SKUs = new List<string>() { $"mtc-{index+1}" }
            });
        }

        await promotionAppService.CreatePromotionAsync(new CreatePromotionRequest()
        {
            Name = $"Promotion {DateTime.Now.ToString("HH:mm")}",
            ExternalId = promotionExternalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(30),
            UnitMeasurement = "kg",
            Attributes = promotionAttributesRequest,
            Rules = new List<CreatePromotionRuleRequest>() {
                new CreatePromotionRuleRequest() {
                    Percentage = 2,
                    TotalAttributes = 1,
                    GreaterEqualValue = 3
                },
                new CreatePromotionRuleRequest() {
                    Percentage = 5,
                    TotalAttributes = 2,
                    GreaterEqualValue = 10                    
                }
            },
            Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SP" } } }
        }, promotionOrderRequest.SellerId);

        await promotionAppService.AddCnpjsAsync(new CreatePromotionCnpjRequest()
        {
            Cnpjs = new List<string>() { $"{customer?.Cnpj}" },
            ExternalId = promotionExternalId
        }, promotionOrderRequest.SellerId);
    }

}
