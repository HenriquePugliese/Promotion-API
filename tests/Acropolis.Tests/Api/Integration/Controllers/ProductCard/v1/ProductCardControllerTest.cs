using Acropolis.Api;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Tests.Helpers;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace Acropolis.Tests.Api.Integration.Controllers.ProductCard.v1;
public class ProductCardControllerTest : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;

    public ProductCardControllerTest()
    {
        _client = GetTestAppClient();
    }

    [Fact(DisplayName = "Should return 'HasPromotion' false when try to find invalid productid")]
    public async Task ProductCardPromotion_InvalidProductIdValidCNPJ_ShouldReturnOkAndHasPromotionFalse()
    {
        //Arrange
        var command = new ProductCardRequest("72527073000147")
        {
            ProductsList = new List<ProductSellerRequest>()
            {
                new ProductSellerRequest()
                {
                    Id = "",
                    SellerId= Guid.NewGuid(),
                }
            }
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/card", command);
        var content = await response.Content.ReadAsStringAsync();
        var responseConverted = JsonConvert.DeserializeObject<ProductCardResponse>(content);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.HasDiscount).Should().Equal(false);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.PromotionMessage).Should().Equal("");
    }

    [Fact(DisplayName = "Should return 'HasPromotion' true when try to find a valid productid")]
    public async Task ProductCardPromotion_ValidProductIdValidCNPJ_ShouldReturnOkAndHasPromotionFalse()
    {
        //Arrange
        var externalId = "f38436ba-c2c8-4d3e-9ed9-fcd9cbd5c2af";
        var productId = "a828401f-8697-435f-b764-d4fad8d13098";
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var cnpj = "72527073000147";
        var commandCNPJ = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                cnpj
            }
        };
        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);
        
        var command = new ProductCardRequest(cnpj)
        {
            ProductsList = new List<ProductSellerRequest>()
            {
                new ProductSellerRequest()
                {
                    Id = productId,
                    SellerId= sellerId 
                }
            }
        };

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);

        await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", commandCNPJ);

        await CreateProduct(new Guid(productId), sellerId);

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/card", command);
        var content = await response.Content.ReadAsStringAsync();
        var responseConverted = JsonConvert.DeserializeObject<ProductCardResponse>(content);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.HasDiscount).Should().Equal(true);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.PromotionMessage).Should().Equal("Produto da campanha D+");
    }

    [Fact(DisplayName = "Should return 'HasPromotion' false when try to find an invalid productid")]
    public async Task ProductCardPromotion_ValidProductIdInvalidCNPJ_ShouldReturnOkAndHasPromotionFalse()
    {
        //Arrange
        var externalId = Guid.NewGuid().ToString();
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var commandCNPJ = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                "72527073000147"
            }
        };
        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        var command = new ProductCardRequest("72527073000149")
        {
            ProductsList = new List<ProductSellerRequest>()
            {
                new ProductSellerRequest()
                {
                    Id = commandPromotion.Attributes.First().SKUs.First(),
                    SellerId= sellerId
                }
            }
        };

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);

        _ = await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", commandCNPJ);
        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/card", command);
        var content = await response.Content.ReadAsStringAsync();
        var responseConverted = JsonConvert.DeserializeObject<ProductCardResponse>(content);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.HasDiscount).Should().Equal(false);
    }

    [Fact(DisplayName = "Should return 'HasPromotion' false when try to find an invalid sellerid")]
    public async Task ProductCardPromotion_InvalidSeller_ShouldReturnOkAndHasPromotionFalse()
    {
        //Arrange
        var sellerId = Guid.Empty;
        var command = new ProductCardRequest("72527073000147")
        {
            ProductsList = new List<ProductSellerRequest>()
            {
                new ProductSellerRequest()
                {
                    Id = new Guid().ToString(),
                    SellerId= Guid.Empty
                }
            }
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/card", command);
        var content = await response.Content.ReadAsStringAsync();
        var responseConverted = JsonConvert.DeserializeObject<ProductCardResponse>(content);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.HasDiscount).Should().Equal(false);
    }

    [Fact(DisplayName = "Should return 'HasPromotion' true when try to find a valid sellerid")]
    public async Task ProductCardPromotion_ValidSellerId_ShouldReturnOkAndHasPromotionTrue()
    {
        //Arrange
        var externalId = "f38436ba-c2c8-4d3e-9ed9-fcd9cbd5c2af";
        var productId = "a828401f-8697-435f-b764-d4fad8d13098";
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var cnpj = "72527073000147";
        var commandCNPJ = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                cnpj
            }
        };
        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        var command = new ProductCardRequest(cnpj)
        {
            ProductsList = new List<ProductSellerRequest>()
            {
                new ProductSellerRequest()
                {
                    Id = productId,
                    SellerId= sellerId
                }
            }
        };

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);

        await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", commandCNPJ);

        await CreateProduct(new Guid(productId), sellerId);

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/card", command);
        var content = await response.Content.ReadAsStringAsync();
        var responseConverted = JsonConvert.DeserializeObject<ProductCardResponse>(content);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.HasDiscount).Should().Equal(true);
        responseConverted.Products.Where(p => p.ProductId == command.ProductsList.First().Id).Select(p => p.PromotionMessage).Should().Equal("Produto da campanha D+");
    }

    private async Task CreateProduct(Guid productId, Guid sellerId)
    {
        using var scope = ServiceProvider?.CreateScope();

        if (scope is null)
            return;

        var productAppService = scope.ServiceProvider.GetRequiredService<IProductAppService>();

        await productAppService.CreateProduct(new CreateProductRequest()
        {
            Id = productId,
            SellerId = sellerId,
            MaterialCode = "sku1",
            Name = "Produto 3",
            Status = 1,
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10
        });
    }
}
