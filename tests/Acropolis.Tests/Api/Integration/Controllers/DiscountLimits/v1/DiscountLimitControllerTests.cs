using Acropolis.Api;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Acropolis.Tests.Api.Integration.Controllers.DiscountLimits.v1;

public class DiscountLimitControllerTests : IntegrationTestBaseWithFakeJWT<Startup>
{
    private readonly HttpClient _client;
    public DiscountLimitControllerTests()
    {
        _client = GetTestAppClient();
    }

    private void SetInvalidUser()
    {
        DefaultFakeUserFilter.Claims.Clear();
        DefaultFakeUserFilter.Claims.Add(new Claim(ClaimTypes.Role, "Customer"));
    }

    [Fact(DisplayName = "Should return status 'BadRequest' when create a discount limit request invalid")]
    public async Task CreateDiscountLimit_CreateADiscountLimitInvalid_ShouldReturnStatusBadRequest()
    {
        //Arrange
        CreateDiscountLimitRequest? command = null;

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'UnprocessableEntity' when create a discount limit request without discount limits")]
    public async Task CreateDiscountLimit_CreateADiscountLimitWithoutDiscountLimits_ShouldReturnStatusUnprocessableEntity()
    {
        //Arrange
        var command = new List<CreateDiscountLimitRequest>();

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Should return invalid response when create discount limit using empty sellerid")]
    public async Task CreateDiscountLimit_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        var command = new List<CreateDiscountLimitRequest>();
        SetInvalidUser();

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return status 'Created' when create a discount limit")]
    public async Task CreateDiscountLimit_CreateADiscountLimitValid_ShouldReturnStatusCreated()
    {
        //Arrange
        var command = new List<CreateDiscountLimitRequest>()
        {
            new CreateDiscountLimitRequest()
            {
                Cnpj = "48.046.585/0001-07",
                Percent = 14
            },
            new CreateDiscountLimitRequest()
            {
                Cnpj = "72.527.073/0001-47",
                Percent = 6
            },
            new CreateDiscountLimitRequest()
            {
                Cnpj = "78.613.663/0001-79",
                Percent = 20
            }
        };

        //Act
        var response = await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "Should return invalid response when get a discount limit not found")]
    public async Task GetDiscountLimit_GetDiscountLimitNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var cnpj = "00000000000001";

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Should return invalid response when get a discount limit woth empty sellerid")]
    public async Task GetDiscountLimit_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        var cnpj = "00000000000001";
        SetInvalidUser();

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return valid response when get a existing discount limit")]
    public async Task GetDiscountLimit_GetExistingDiscountLimit_ShouldReturnValidResponse()
    {
        //Arrange
        var command = new List<CreateDiscountLimitRequest>()
        {
            new CreateDiscountLimitRequest()
            {
                Cnpj = "92.723.795/0001-84",
                Percent = 12
            }
        };

        await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);

        var cnpj = "92723795000184";

        //Act
        var response = await _client.GetAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Should return status 'Unauthorized' when remove a discount limit with invalid user")]
    public async Task RemoveDiscountLimit_RemoveADiscountLimitNotFound_ShouldReturnInvalidResponse()
    {
        //Arrange
        var cnpj = "00000000000001";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Should return invalid response when remove a discount limit with empty sellerid")]
    public async Task RemoveDiscountLimit_EmptySellerId_ShouldReturnInvalidResponse()
    {
        //Arrange
        var cnpj = "00000000000001";
        SetInvalidUser();

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Should return invalid response when remove a discount limit with cnpj empty")]
    public async Task RemoveDiscountLimit_RemoveADiscountLimitWitCnpjEmpty_ShouldReturnInvalidResponse()
    {
        //Arrange
        var cnpj = string.Empty;

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return valid response when remove a existing discount limit")]
    public async Task RemoveDiscountLimit_RemoveAExistingDiscountLimit_ShouldReturnValidResponse()
    {
        //Arrange
        var command = new List<CreateDiscountLimitRequest>()
        {
            new CreateDiscountLimitRequest()
            {
                Cnpj = "92.723.795/0001-84",
                Percent = 12
            }
        };

        await _client.PostAsJsonAsync("/v1/desconto-mais/discount-limit", command);
        var cnpj = "92723795000184";

        //Act
        var response = await _client.DeleteAsync($"/v1/desconto-mais/discount-limit/{cnpj}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    
}
