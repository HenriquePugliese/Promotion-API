using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;
using Moq;

namespace Acropolis.Tests.Application.Features.Promotions.Services;

public class PromotionAppServiceTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should return invalid response when create a promotion without attributes")]
    public async Task CreatePromotion_CreateAPromotionWithoutAttributes_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();

        // act
        var command = PromotionRequestHelper.CreatePromotionRequestWithoutAttributes();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("atributo"));
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion without rules")]
    public async Task CreatePromotion_CreateAPromotionWithoutRules_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();

        // act
        var command = PromotionRequestHelper.CreatePromotionRequestWithoutRules();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("regra"));
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion with date end less than current date")]
    public async Task CreatePromotion_CreateAPromotionWithDateEndLessThanCurrentDate_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();

        // act
        var command = PromotionRequestHelper.CreatePromotionRequestWithDateEndInvalid();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("data final"));
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion with external id duplicated")]
    public async Task CreatePromotion_CreateAPromotionWithExternalIdDuplicated_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();

        promotionRepositoryMock.Setup(repository => repository.HasPromotionAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

        // act
        var command = PromotionRequestHelper.CreateValidPromotionRequest();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("identificador externo"));
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion with percentage greather than default")]
    public async Task CreatePromotion_CreateAPromotionWithPercentageGreatherThanDefault_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();

        // act
        var command = PromotionRequestHelper.CreatePromotionRequestWithPercentageInvalid();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("percentual"));
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion with empty name")]
    public async Task CreatePromotion_EmptyName_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();
        var command = PromotionRequestHelper.CreateValidPromotionRequest();
        command.Name = string.Empty;

        // act
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description == "Nome da promoção é obrigatório.");
    }

    [Fact(DisplayName = "Should return invalid response when create a promotion with minvalue DtStart")]
    public async Task CreatePromotion_MinvalueDtStart_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();
        var command = PromotionRequestHelper.CreateValidPromotionRequest();
        command.DtStart = DateTime.MinValue;

        // act
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description == "Datas da promoção são obrigatórias.");
    }

    [Fact(DisplayName = "Should return valid response when create a promotion")]
    public async Task CreatePromotion_CreateAValidPromotion_ShouldReturnValidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();

        // act
        var command = PromotionRequestHelper.CreateValidPromotionRequest();
        var (response, createdPromotion) = await service.CreatePromotionAsync(command, SellerId_Votorantim);

        // assert
        createdPromotion.Should().NotBeNull();
        createdPromotion?.Id.Should().NotBe(Guid.Empty);
        createdPromotion?.Attributes.Should().NotBeEmpty().And.HaveCount(1);
        createdPromotion?.Attributes.First().SKUs.Should().NotBeEmpty();
        createdPromotion?.Parameters.Should().NotBeEmpty().And.HaveCount(1);
        createdPromotion?.Rules.Should().NotBeEmpty().And.HaveCount(1);
        response?.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "Should return invalid response when get a promotion not found")]
    public async Task GetPromotion_GetPromotionNotFound_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(null));

        // act
        var (response, promotionResponse) = await service.GetPromotionAsync(promotionExternalId, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description == "Promotion not found");
    }

    [Fact(DisplayName = "Should return valid response when get a existing promotion")]
    public async Task GetPromotion_GetExistingPromotion_ShouldReturValidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);

        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(
            Task.FromResult<Promotion?>(validPromotion)
            );

        // act
        var (response, promotionResponse) = await service.GetPromotionAsync(promotionExternalId, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeTrue();
        promotionResponse.Should().NotBeNull();
        promotionResponse?.DtStart.Should().Be(validPromotion.DtStart);
        promotionResponse?.DtEnd.Should().Be(validPromotion.DtEnd);
        promotionResponse?.Name.Should().Be(validPromotion.Name);
        promotionResponse?.ExternalId.Should().Be(validPromotion.ExternalId);
        promotionResponse?.StatusName.Should().Be("Active");
        promotionResponse?.Parameters?.Count.Should().Be(1);
        promotionResponse?.Rules?.Count.Should().Be(1);
        promotionResponse?.Attributes?.Count.Should().Be(1);
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion with external id empty")]
    public async Task RemovePromotion_RemoveAPromotionWithExternalIdEmpty_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = string.Empty;
        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(null));

        // act
        var command = new RemovePromotionRequest(promotionExternalId);
        var response = await service.RemovePromotionAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.ToLower().Contains("identificador externo"));
    }

    [Fact(DisplayName = "Should return invalid response when remove a promotion not found")]
    public async Task RemovePromotion_RemoveAPromotionNotFound_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(null));

        // act
        var command = new RemovePromotionRequest(promotionExternalId);
        var response = await service.RemovePromotionAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description == "Promotion not found");
    }

    [Fact(DisplayName = "Should return valid response when remove a existing promotion")]
    public async Task RemovePromotion_RemoveAExistingPromotion_ShouldReturValidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);

        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(
            Task.FromResult<Promotion?>(validPromotion)
            );

        // act
        var command = new RemovePromotionRequest(promotionExternalId);
        var response = await service.RemovePromotionAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "Should return invalid response when add cpnjs with no cnpj")]
    public async Task AddCnpjs_NoCnpj_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim, promotionExternalId);
        var cnpjs = new List<string>();

        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(validPromotion));

        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().BeNull();
        response?.IsValid().Should().BeFalse();
    }

    [Fact(DisplayName = "Should return invalid response when add cpnjs with more than one thousand cnpj")]
    public async Task AddCnpjs_MoreThanOneThousandCnpj_ShouldReturnInvalidResponse()
    {
        // arrange
        var (promotionRepositoryMock, service) = PromotionContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim, promotionExternalId);
        var cnpjs = new List<string>();

        for (int i = 10; i <= 1201; i++)
        {
            cnpjs.Add("725270730001" + i);
        }

        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(validPromotion));

        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().BeNull();
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.Contains("No máximo"));
    }

    [Fact(DisplayName = "Should return valid response when add cpnjs")]
    public async Task AddCnpjs_AddAValidCNPJs_ShouldReturnValidResponse()
    {
        // arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, service) = PromotionCnpjContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim, promotionExternalId);
        var cnpjs = new List<string>() { "27.366.459/0001-60", "72527073000147" };
        
        promotionCnpjRepositoryMock.Setup(r => r.FindAsync(It.IsAny<PromotionCnpjParameters>(), It.IsAny<Guid>())).Returns(Task.FromResult<IPagedList<PromotionCnpjResponse>>(new PagedList<PromotionCnpjResponse>(new List<PromotionCnpjResponse>(), 0, 1, 1)));
        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(validPromotion));
        
        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().NotBeNull();
        createdPromotionCnpj?.ExternalId.Should().Be(promotionExternalId);
        createdPromotionCnpj?.Cnpjs.Should().NotBeEmpty().And.HaveCount(2);
        response?.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "Should return invalid response when add invalid cpnjs")]
    public async Task AddCnpjs_AddInvalidCNPJs_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();
        var promotionExternalId = "ext-ok-1";
        var cnpjs = new List<string>() { "00.36ss346.459/0001-60", "9900900000009383" };

        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().BeNull();
        createdPromotionCnpj?.Cnpjs.Should().BeEmpty().And.HaveCount(0);
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("cnpj"));
    }

    [Fact(DisplayName = "Should return valid response when add a existing cpnj")]
    public async Task AddCnpjs_AddExistingCNPJ_ShouldReturnValidResponse()
    {
        // arrange
        var (promotionRepositoryMock, promotionCnpjRepositoryMock, service) = PromotionCnpjContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var validPromotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim, promotionExternalId);
        var cnpjs = new List<string>() { "27.366.459/0001-60", "72527073000147" };
        var promotionCnpjs = new PagedList<PromotionCnpjResponse>(new List<PromotionCnpjResponse>()
        {
            new PromotionCnpjResponse(promotionExternalId, new List<string>(){ "72527073000147" })
        }, cnpjs.Count, 1, 50);

        promotionRepositoryMock.Setup(r => r.GetByExternalIdAsync(promotionExternalId, SellerId_Votorantim)).Returns(Task.FromResult<Promotion?>(validPromotion));
        promotionCnpjRepositoryMock.Setup(r => r.FindAsync(It.IsAny<PromotionCnpjParameters>(), It.IsAny<Guid>())).Returns(Task.FromResult<IPagedList<PromotionCnpjResponse>>(promotionCnpjs));

        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().NotBeNull();
        createdPromotionCnpj?.Cnpjs.Should().NotBeEmpty().And.HaveCount(1);
        createdPromotionCnpj?.Cnpjs.Should().Contain(cnpj => cnpj.Equals("27366459000160"));
        response?.IsValid().Should().BeTrue();        
    }

    [Fact(DisplayName = "Should return invalid response when add cnpj with invalid external id")]
    public async Task AddCnpjs_AddInvalidExternalId_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();
        var cnpjs = new List<string>() { "27.366.459/0001-60", "72527073000147" };
        var promotionExternalId = string.Empty;

        // act
        var command = new CreatePromotionCnpjRequest() { ExternalId = promotionExternalId, Cnpjs = cnpjs };
        var (response, createdPromotionCnpj) = await service.AddCnpjsAsync(command, SellerId_Votorantim);

        // assert
        createdPromotionCnpj.Should().BeNull();
        createdPromotionCnpj?.Cnpjs.Should().BeEmpty().And.HaveCount(0);
        response?.IsValid().Should().BeFalse();
        response?.Notifications.Should().Contain(n => n.Description.ToLower().Contains("identificador externo"));
    }

    [Fact(DisplayName = "Should return invalid response when remove cnpj from non-existent promotion")]
    public async Task RemoveCnpj_RemoveCnpjFromNonExistentPromotion_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, service) = PromotionContextMock();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var cnpj = "72527073000147";

        promotionCnpjRepositoryMock.Setup(promotion => promotion.GetByAsync(promotionExternalId,cnpj, SellerId_Votorantim)).Returns(Task.FromResult<PromotionCnpj?>(null));

        // act
        var command = new RemovePromotionCnpjRequest(promotionExternalId, cnpj);
        var response = await service.RemoveCnpjAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
        response.Notifications.Should().Contain(n => n.Description.ToLower().Contains("inexistente"));
    }

    [Fact(DisplayName = "Should return valid response when remove a existing cnpj")]
    public async Task RemoveCnpj_RemoveAExistingPromotionCnpj_ShouldReturnValidResponse()
    {
        // arrange
        var (_, promotionCnpjRepositoryMock, service) = PromotionCnpjContextMock();
        var promotionExternalId = $"{Guid.NewGuid()}-test";
        var cnpj = "72527073000147";
        var validPromotionCnpj = new PromotionCnpj(promotionExternalId, cnpj, SellerId_Votorantim);        
        
        promotionCnpjRepositoryMock.Setup(promotion => promotion.GetByAsync(promotionExternalId, cnpj, SellerId_Votorantim)).Returns(Task.FromResult<PromotionCnpj?>(validPromotionCnpj));

        // act
        var command = new RemovePromotionCnpjRequest(promotionExternalId, cnpj);
        var response = await service.RemoveCnpjAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "Should return invalid response when remove cnpj with empty externalid")]
    public async Task RemoveCnpj_EmptyExternalid_ShouldReturnInvalidResponse()
    {
        // arrange
        var (_, promotionCnpjRepositoryMock, service) = PromotionCnpjContextMock();
        var promotionExternalId = "";
        var cnpj = "72527073000147";
        var validPromotionCnpj = new PromotionCnpj(promotionExternalId, cnpj, SellerId_Votorantim);

        promotionCnpjRepositoryMock.Setup(promotion => promotion.GetByAsync(promotionExternalId, cnpj, SellerId_Votorantim)).Returns(Task.FromResult<PromotionCnpj?>(validPromotionCnpj));

        // act
        var command = new RemovePromotionCnpjRequest(promotionExternalId, cnpj);
        var response = await service.RemoveCnpjAsync(command, SellerId_Votorantim);

        // assert
        response.IsValid().Should().BeFalse();
    }

    private static (Mock<IPromotionRepository> RepositoryMock, IPromotionAppService Service) PromotionContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        var promotionService = new PromotionAppService(unitOfWorkMock.Object, promotionRepositoryMock.Object, promotionCnpjRepositoryMock.Object);

        return (promotionRepositoryMock, promotionService);
    }

    private static (Mock<IPromotionRepository> RepositoryMock, Mock<IPromotionCnpjRepository> RepositoryCnpjMock, IPromotionAppService Service) PromotionCnpjContextMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        var promotionCnpjRepositoryMock = new Mock<IPromotionCnpjRepository>();

        unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        var promotionService = new PromotionAppService(unitOfWorkMock.Object, promotionRepositoryMock.Object, promotionCnpjRepositoryMock.Object);

        return (promotionRepositoryMock, promotionCnpjRepositoryMock, promotionService);
    }
}
