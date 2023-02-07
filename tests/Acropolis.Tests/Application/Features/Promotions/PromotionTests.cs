using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Enums;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionTests
{

    [Fact(DisplayName = "Should create active promotion")]
    public void CreatePromotion_Active_ShoudCreateActivePromotion()
    {
        //Arrange
        var promotionRequest = PromotionRequestHelper.CreateValidPromotionRequest();
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

        //Act
        var newPromotion = new Promotion(promotionRequest, sellerId);

        //Assert
        newPromotion.Id.Should().NotBe(Guid.Empty);
        newPromotion.Name.Should().Be(promotionRequest.Name);
        newPromotion.Status.Should().Be(PromotionStatus.Active);
    }

    [Fact(DisplayName = "Should create promotion with specific id")]
    public void CreatePromotion_WithSpecificId_ShoudCreatePromotion()
    {
        //Arrange
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var promotionRequest = PromotionRequestHelper.CreateValidPromotionRequest();
        var promotionId = Guid.NewGuid();

        //Act
        var newPromotion = new Promotion(promotionRequest, sellerId, promotionId);

        //Assert
        newPromotion.Id.Should().Be(promotionId);
    }

}
