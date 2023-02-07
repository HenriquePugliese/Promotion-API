using Acropolis.Api;
using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Tests.Helpers;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Acropolis.Tests.Api.Integration.Controllers.Promotions.v1;

public class PromotionControllerTests : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;

    public PromotionControllerTests()
    {
        _client = GetTestAppClient();
    }

    private void SetInvalidUser()
    {
        DefaultFakeUserFilter.Claims.Clear();
        DefaultFakeUserFilter.Claims.Add(new Claim(ClaimTypes.Role, "Customer"));
    }

    [Fact(DisplayName = "Should return status 'BadRequest' when create a promotion invalid")]
    public async Task CreatePromotion_CreateAPromotionInvalid_ShouldReturnStatusBadRequest()
    {
        //Arrange
        var command = new CreatePromotionRequest()
        {
            Name = "Promotion test invalid"
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'UnprocessableEntity' when create a promotion with external id empty")]
    public async Task CreatePromotion_CreateAPromotionWithExternalIdEmpty_ShouldReturnStatusUnprocessableEntity()
    {
        //Arrange
        var command = PromotionRequestHelper.CreateInvalidPromotionRequest();

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        content.ToLower().Should().Contain("identificador externo");
    }

    [Fact(DisplayName = "Should return status 'BadRequest' when create a promotion with invalid attributes id")]
    public async Task CreatePromotion_WithInvalidAttributesId_ShouldReturnStatusBadRequest()
    {
        //Arrange
        var attributesId = "attrIdInvalid";
        var command = PromotionRequestHelper.CreateValidPromotionRequest(null, attributesId);

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("AttributesId");
    }

    [Fact(DisplayName = "Should return invalide response when create a promotion with empty sellerid")]
    public async Task CreatePromotion_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var command = PromotionRequestHelper.CreateValidPromotionRequest();

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'Created' when create a promotion")]
    public async Task CreatePromotion_CreateAPromotionValid_ShouldReturnStatusCreated()
    {
        //Arrange
        var command = PromotionRequestHelper.CreateValidPromotionRequest();

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "Should return paginated results when find promotions")]
    public async Task FindPromotions_PaginatedFind_ShouldReturnPaginatedResults()
    {
        //Arrange
        var page = 1;
        var pageSize = 2;
        var responseString = await _client.GetStringAsync($"/v1/desconto-mais/?pageIndex={page}&pageSize={pageSize}");

        //Act
        var response = JsonConvert.DeserializeObject<PagedList<PromotionResponse>>(responseString);

        //Assert
        response!.Data.Should().HaveCount(pageSize);
        response!.PageIndex.Should().Be(page);
        response!.PageSize.Should().Be(pageSize);
    }

    [Fact(DisplayName = "Should return invalid response when find promotions with empty sellerid")]
    public async Task FindPromotions_EmptySelleriId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var page = 1;
        var pageSize = 2;

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/?pageIndex={page}&pageSize={pageSize}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return valid response when find promotions by name")]
    public async Task FindPromotions_FindPromotionByName_ShouldReturnValidResponse()
    {
        //Arrange
        var promotionName = "Promotion Test 2";

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/?name={promotionName}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Should return invalid response when get a promotion not found")]
    public async Task GetPromotion_GetPromotionNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var promotionExternalId = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/{promotionExternalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Should return invalid response when get promotion with empty sellerid")]
    public async Task GetPromotion_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var promotionExternalId = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/{promotionExternalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return valid response when get a existing promotion")]
    public async Task GetPromotion_GetExistingPromotion_ShouldReturnValidResponse()
    {
        //Arrange
        var command = PromotionRequestHelper.CreateValidPromotionRequest();
        var responseCreate = await _client.PostAsJsonAsync("/v1/desconto-mais/", command);
        var content = await responseCreate.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<PromotionResponse>(content);

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/{responseContent.ExternalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion not found")]
    public async Task RemovePromotion_RemoveAPromotionNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var externalId = "ext-invalid-1z";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/{externalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion with external id empty")]
    public async Task RemovePromotion_RemoveAPromotionWithExternalIdEmpty_ShouldReturnInvalidResponse()
    {
        //Arrange
        string externalId = string.Empty;

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/{externalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when remove promotion with empty sellerid")]
    public async Task RemovePromotion_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var externalId = "ext-ok-50";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/{externalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return valid response when remove a existing promotion")]
    public async Task RemovePromotion_RemoveAExistingPromotion_ShouldReturnValidResponse()
    {
        //Arrange
        var externalId = "ext-ok-50";
        var command = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        await _client.PostAsJsonAsync("/v1/desconto-mais/", command);

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/{externalId}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
