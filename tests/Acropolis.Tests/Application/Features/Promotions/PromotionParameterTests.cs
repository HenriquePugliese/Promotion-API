using Acropolis.Application.Features.Promotions;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionParameterTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should create promotion parameter with segment kind")]
    public void CreatePromotionParameter_WithSegmentKind_ShoudCreatePromotionParameter()
    {
        //Arrange
        var promotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);
        var kind = Acropolis.Application.Features.Promotions.Enums.PromotionParameter.Segmento;
        var value = "seg123";

        //Act
        var newPromotionParameter = new PromotionParameter(kind, value, promotion);

        //Assert
        newPromotionParameter.Kind.HasValue.Should().BeTrue();
        newPromotionParameter.Value.Should().Be(value);
        newPromotionParameter.Kind.Should().Be(kind);
        newPromotionParameter.PromotionId.Should().Be(promotion.Id);
    }

}