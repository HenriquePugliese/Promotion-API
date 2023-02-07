using Acropolis.Api;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Acropolis.Tests.Api.Integration.Controllers.PromotionsProducts.v1;

public class GetPromotionProductControllerTests : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;

    public GetPromotionProductControllerTests()
    {
        _client = GetTestAppClient();
    }

    [Fact(DisplayName = "Should return invalid response when get a product not found")]
    public async Task GetPromotionProductIncentiveList_GetProductNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var cnpj = "97194593000106";

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/incentivelist/{productId}/{sellerId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Should return invalid response when get a product with different promotion parameters")]
    public async Task GetPromotionProductIncentiveList_GetProductWithDifferentPromotionParameters_ShouldReturnInvalidResponse()
    {
        //Arrange
        var productId = new Guid("62D91665-3B58-4FA0-BE83-2C40302D9C0A");
        var sellerId = new Guid("87720FA0-5D9E-4149-838A-9CE21DDC9033");
        var attributeId = new Guid("2C7A038F-19FC-4B99-94BB-E79DCDCE1F11");
        var cnpj = "72527073000147";
        var promotionParameter = "AC";

        await CreateProductIncentiveList(productId, sellerId, cnpj, attributeId, promotionParameter);

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/incentivelist/{productId}/{sellerId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return valid response when get a product incentive list")]
    public async Task GetPromotionProductIncentiveList_GetExistingProductIncentiveList_ShouldReturnValidResponse()
    {
        //Arrange
        var productId = new Guid("62D91665-3B58-4FA0-BE83-2C40302D9C0A");
        var sellerId = new Guid("87720FA0-5D9E-4149-838A-9CE21DDC9033");
        var attributeId = new Guid("2C7A038F-19FC-4B99-94BB-E79DCDCE1F11");
        var cnpj = "97194593000106";

        await CreateProductIncentiveList(productId, sellerId, cnpj, attributeId);

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/incentivelist/{productId}/{sellerId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task CreateProductIncentiveList(Guid productId, Guid sellerId, string cnpj, Guid attributeId, string? promotionParameter = null)
    {
        using var scope = ServiceProvider?.CreateScope();

        if (scope is null)
            return;

        var productAppService = scope.ServiceProvider.GetRequiredService<IProductAppService>();
        var attributeAppService = scope.ServiceProvider.GetRequiredService<IAttributeAppService>();
        var promotionAppService = scope.ServiceProvider.GetRequiredService<IPromotionAppService>();
        var promotionExternalId = Guid.NewGuid().ToString();

        var createPromotionRequest = new CreatePromotionRequest()
        {
            Name = "Promotion dfasd",
            ExternalId = promotionExternalId,
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(30),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attributeId.ToString(), Qtd = 1 } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 3 } }
        };

        if (!string.IsNullOrWhiteSpace(promotionParameter))
            createPromotionRequest.Parameters = new List<CreatePromotionParameterRequest>()
            {
                new CreatePromotionParameterRequest()
                {
                    UF = new string[]{ $"{promotionParameter}" }
                }
            };

        await promotionAppService.CreatePromotionAsync(createPromotionRequest, sellerId);

        await promotionAppService.AddCnpjsAsync(new CreatePromotionCnpjRequest()
        {
            Cnpjs = new List<string>() { cnpj },
            ExternalId = promotionExternalId
        }, sellerId);

        await productAppService.CreateProduct(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "mtc-1",
            Name = "Produto 1",
            Status = 1,
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10
        });

        await attributeAppService.CreateAttributeConsumer(new CreateAttributeRequest()
        {
            ProductId = productId,
            AttributeKey = "attr-1",
            AttributeKeyId = Guid.NewGuid(),
            AttributeKeyDescription = "attr-1",
            AttributeKeyLabel = "attr-1",
            AttributeKeyIsBeginOpen = true,
            AttributeKeyStatus = 1,
            AttributeKeyType = Acropolis.Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = "attr-1",
            AttributeValueId = attributeId,
            AttributeValueLabel = "attr-1",
            AttributeValueStatus = 1
        });
    }
}
