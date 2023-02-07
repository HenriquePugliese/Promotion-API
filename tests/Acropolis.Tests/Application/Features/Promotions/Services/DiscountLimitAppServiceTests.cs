using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Services;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using FluentAssertions;
using Moq;

namespace Acropolis.Tests.Application.Features.Promotions.Services;

public class DiscountLimitAppServiceTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should return invalid response when create a discount limit with percent invalid")]
    public async Task CreateDiscountLimit_CreateADiscountLimitWithPercentInvalid_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = DiscountLimitContextMock();
        var command = new List<CreateDiscountLimitRequest>(){
            new CreateDiscountLimitRequest()
            {
                Cnpj = "46.420.948/0001-05",
                Percent = 0
            },
            new CreateDiscountLimitRequest()
            {
                Cnpj = "46.420.948/0001-05",
                Percent = 105
            }
        };

        // act
        var (response, createdDiscountLimit) = await service.CreateDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        createdDiscountLimit.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("percentual de desconto"));
    }

    [Fact(DisplayName = "Should return invalid response when create a discount limit with cnpj invalid")]
    public async Task CreateDiscountLimit_CreateADiscountLimitWithCnpjInvalid_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = DiscountLimitContextMock();
        var command = new List<CreateDiscountLimitRequest>(){
            new CreateDiscountLimitRequest()
            {
                Cnpj = "00.000.001/0001-00",
                Percent = 10
            }
        };

        // act
        var (response, createdDiscountLimit) = await service.CreateDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        createdDiscountLimit.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("cnpj"));
    }

    [Fact(DisplayName = "Should return invalid response when create a discount limit with cnpjs amout exceeds limit")]
    public async Task CreateDiscountLimit_CreateADiscountLimitWithCnpjsAmountExceedsLimit_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = DiscountLimitContextMock();
        var cnpjsMaxAmount = 50;
        var cnpjsAmount = cnpjsMaxAmount + 1;
        var command = new List<CreateDiscountLimitRequest>();

        for (var index = 0; index < cnpjsAmount; index++)
            command.Add(new CreateDiscountLimitRequest() {  Cnpj = "46664696000151" , Percent = 10});

        // act
        var (response, createdDiscountLimit) = await service.CreateDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        createdDiscountLimit.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("cnpjs"));
    }

    [Fact(DisplayName = "Should return invalid response when create a discount limit with empty cnpj")]
    public async Task CreateDiscountLimit_EmptyCnpj_ShouldReturnInvalidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "";
        var percent = 10;
        var command = new List<CreateDiscountLimitRequest>(){
            new CreateDiscountLimitRequest()
            {
                Cnpj = cnpj,
                Percent = percent
            }
        };

        discountLimitRepositoryMock
            .Setup(repository => repository.GetCnpjsWithDiscountLimitsAsync(It.IsAny<IEnumerable<string>>(), SellerId_Votorantim))
            .Returns(Task.FromResult<IEnumerable<string>>(new List<string>()));

        // act
        var (response, createdDiscountLimit) = await service.CreateDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        createdDiscountLimit.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.Contains("CNPJ é obrigatório."));
    }

    [Fact(DisplayName = "Should return valid response when create a discount limit")]
    public async Task CreateDiscountLimit_CreateAValidDiscountLimit_ShouldReturnValidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "46664696000151";
        var percent = 10;
        var command = new List<CreateDiscountLimitRequest>(){
            new CreateDiscountLimitRequest()
            {
                Cnpj = cnpj,
                Percent = percent
            }
        };

        discountLimitRepositoryMock
            .Setup(repository => repository.GetCnpjsWithDiscountLimitsAsync(It.IsAny<IEnumerable<string>>(), SellerId_Votorantim))
            .Returns(Task.FromResult<IEnumerable<string>>(new List<string>()));

        // act
        var (response, createdDiscountLimit) = await service.CreateDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        createdDiscountLimit.Should().NotBeNull();
        createdDiscountLimit?.FirstOrDefault()?.Percent.Should().Be(percent);
        createdDiscountLimit?.FirstOrDefault()?.Cnpj.Should().Be(cnpj);
        response?.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "Should return invalid response when get a discount limit not found")]
    public async Task GetDiscountLimit_GetDiscountLimitNotFound_ShouldReturnInvalidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "00000090000000";

        discountLimitRepositoryMock
            .Setup(repository => repository.GetByCnpjAsync(It.IsAny<string>(), SellerId_Votorantim))
            .Returns(Task.FromResult<PromotionCnpjDiscountLimit?>(null));

        // act
        var (response, discountLimitResponse) = await service.GetDiscountLimitAsync(cnpj, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        discountLimitResponse.Should().BeNull();
    }

    [Fact(DisplayName = "Should return valid response when get a existing discount limit")]
    public async Task GetDiscountLimit_GetExistingDiscountLimit_ShouldReturnValidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "92723795000184";

        discountLimitRepositoryMock
            .Setup(repository => repository.GetByCnpjAsync(It.IsAny<string>(), SellerId_Votorantim))
            .Returns(Task.FromResult<PromotionCnpjDiscountLimit?>(new PromotionCnpjDiscountLimit(cnpj,10, SellerId_Votorantim)));

        // act
        var (response, discountLimitResponse) = await service.GetDiscountLimitAsync(cnpj, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeTrue();
        discountLimitResponse.Should().NotBeNull();
        discountLimitResponse?.Cnpj.Should().Be(cnpj);
        discountLimitResponse?.Percent.Should().BeGreaterThan(decimal.Zero);
    }

    [Fact(DisplayName = "Should return invalid response when remove a discount limit with empty cnpj")]
    public async Task RemoveDiscountLimit_RemoveADiscountLimitWithCnpjEmpty_ShouldReturnInvalidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = string.Empty;

        discountLimitRepositoryMock
            .Setup(repository => repository.GetCnpjsWithDiscountLimitsAsync(It.IsAny<IEnumerable<string>>(), SellerId_Votorantim))
            .Returns(Task.FromResult<IEnumerable<string>>(new List<string>()));

        // act
        var command = new RemoveDiscountLimitRequest(cnpj);
        var response = await service.RemoveDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("cnpj"));
    }

    [Fact(DisplayName = "Should return invalid response when remove a discount limit not found")]
    public async Task RemoveDiscountLimit_RemoveAPromotionNotFound_ShouldReturnInvalidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "00000090000000";

        discountLimitRepositoryMock
            .Setup(repository => repository.GetByCnpjAsync(It.IsAny<string>(), SellerId_Votorantim))
            .Returns(Task.FromResult<PromotionCnpjDiscountLimit?>(null));

        // act
        var command = new RemoveDiscountLimitRequest(cnpj);
        var response = await service.RemoveDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("limite de desconto"));
    }

    [Fact(DisplayName = "Should return valid response when remove a existing discount limit")]
    public async Task RemoveDiscountLimit_RemoveAExistingDiscountLimit_ShouldReturValidResponse()
    {
        // arrange
        var (discountLimitRepositoryMock, service) = DiscountLimitContextMock();
        var cnpj = "92723795000184";

        discountLimitRepositoryMock
            .Setup(repository => repository.GetByCnpjAsync(It.IsAny<string>(), SellerId_Votorantim))
            .Returns(Task.FromResult<PromotionCnpjDiscountLimit?>(new PromotionCnpjDiscountLimit(cnpj, 10, SellerId_Votorantim)));

        // act
        var command = new RemoveDiscountLimitRequest(cnpj);
        var response = await service.RemoveDiscountLimitAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeTrue();
    }

    private static (Mock<IPromotionCnpjDiscountLimitRepository> RepositoryMock, IDiscountLimitAppService Service) DiscountLimitContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var discountLimitRepositoryMock = new Mock<IPromotionCnpjDiscountLimitRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        var discountLimitService = new DiscountLimitAppService(unitOfWorkMock.Object, discountLimitRepositoryMock.Object);

        return (discountLimitRepositoryMock, discountLimitService);
    }
}
