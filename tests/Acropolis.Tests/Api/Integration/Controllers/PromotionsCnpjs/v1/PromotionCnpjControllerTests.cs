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

namespace Acropolis.Tests.Api.Integration.Controllers.PromotionsCnpjs.v1;

public class PromotionCnpjControllerTests : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;

    public PromotionCnpjControllerTests()
    {
        _client = GetTestAppClient();
    }

    private void SetInvalidUser()
    {
        DefaultFakeUserFilter.Claims.Clear();
        DefaultFakeUserFilter.Claims.Add(new Claim(ClaimTypes.Role, "Customer"));
    }

    [Fact(DisplayName = "Should return status 'UnprocessableEntity' when create a promotion cnpj invalid")]
    public async Task CreatePromotionCnpj_AddCnpjWithPromotionInvalid_ShouldReturnStatusUnprocessableEntity()
    {
        //Arrange
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = "ext-id-invalid",
            Cnpjs = new List<string>(){
                "72527073000147"
            }
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", command);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        content.ToLower().Should().Contain("promotion not found");
    }

    [Fact(DisplayName = "Should return status 'BadRequest' when create a promotion cnpj with external id empty")]
    public async Task CreatePromotionCnpj_AddCnpjWithExternalIdEmpty_ShouldReturnStatusBadRequest()
    {
        //Arrange
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = string.Empty,
            Cnpjs = new List<string>(){
                "72527073000147"
            }
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'BadRequest' when create a promotion cnpj with empty sellerid")]
    public async Task CreatePromotionCnpj_EmptySellerId_ShouldReturnStatusBadRequest()
    {
        //Arrange
        SetInvalidUser();
        var externalId = Guid.NewGuid().ToString();
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                "72527073000147"
            }
        };
        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'Created' when create promotion cnpj")]
    public async Task CreatePromotionCnpj_AddCnpjWithAPromotionValid_ShouldReturnStatusCreated()
    {
        //Arrange
        var externalId = Guid.NewGuid().ToString();
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                "72527073000147"
            }
        };
        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "Should return invalid response when find promotions cnpjs with empty sellerid")]
    public async Task FindPromotionsCnpjs_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var page = 1;
        var pageSize = 2;
        var externalId = "ext-ok-1";

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/cnpjs/{externalId}?pageIndex={page}&pageSize={pageSize}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return paginated results when find promotions cnpjs")]
    public async Task FindPromotionsCnpjs_PaginatedFind_ShouldReturnPaginatedResults()
    {
        //Arrange
        var page = 1;
        var pageSize = 2;
        var externalId = "ext-ok-1";

        //Act
        var responseString = await _client.GetStringAsync($"/v1/desconto-mais/cnpjs/{externalId}?pageIndex={page}&pageSize={pageSize}");
        var response = JsonConvert.DeserializeObject<PagedList<PromotionCnpjResponse>>(responseString);

        //Assert
        response!.Data.Should().NotBeEmpty();
        response!.PageIndex.Should().Be(page);
        response!.PageSize.Should().Be(pageSize);
    }

    [Fact(DisplayName = "Should return invalid response when find promotions cnpjs with external id empty")]
    public async Task FindPromotionsCnpjs_PaginatedFindWithExternalIdEmpty_ShouldReturnInvalidResponse()
    {
        //Arrange
        var page = 1;
        var pageSize = 1;
        var externalId = string.Empty;

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/cnpjs/{externalId}?pageIndex={page}&pageSize={pageSize}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion cnpj not found")]
    public async Task RemovePromotionCnpj_RemoveACnpjNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var externalId = "ext-ok-1";
        var cnpj = "11112223334444";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/cnpjs/{externalId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion cnpj with external id empty")]
    public async Task RemovePromotionCnpj_RemoveACnpjWithExternalIdEmpty_ShouldReturnInvalidResponse()
    {
        //Arrange
        var externalId = string.Empty;
        var cnpj = "72527073000147";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/cnpjs/{externalId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when remove promotion cnpj with empty sellerid")]
    public async Task RemovePromotionCnpj_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        SetInvalidUser();
        var externalId = Guid.NewGuid().ToString();
        var cnpj = "72527073000147";
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                cnpj
            }
        };

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/cnpjs/{externalId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return valid response when remove a existing promotion cnpj")]
    public async Task RemovePromotionCnpj_RemoveAExistingCnpj_ShouldReturnValidResponse()
    {
        //Arrange
        var externalId = Guid.NewGuid().ToString();
        var cnpj = "72527073000147";
        var command = new CreatePromotionCnpjRequest()
        {
            ExternalId = externalId,
            Cnpjs = new List<string>(){
                cnpj
            }
        };

        var commandPromotion = PromotionRequestHelper.CreateValidPromotionRequest(externalId);

        await _client.PostAsJsonAsync("/v1/desconto-mais/", commandPromotion);
        await _client.PostAsJsonAsync("/v1/desconto-mais/cnpjs", command);

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/cnpjs/{externalId}/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
