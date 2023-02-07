using Acropolis.Application.Features.Promotions;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionRuleTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should create promotion rule with one attribute")]
    public void CreatePromotionRule_WithOneAttribute_ShoudCreatePromotionRule()
    {
        //Arrange
        var promotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);
        var totalAttributes = 1;
        var value = 1m;
        var percentage = 10m;

        //Act
        var newPromotionRule = new PromotionRule(totalAttributes, value, percentage, promotion);

        //Assert
        newPromotionRule.GreaterEqualValue.Should().Be(value);
        newPromotionRule.Percentage.Should().Be(percentage);
        newPromotionRule.TotalAttributes.Should().Be(totalAttributes);
        newPromotionRule.PromotionId.Should().Be(promotion.Id);
    }

}