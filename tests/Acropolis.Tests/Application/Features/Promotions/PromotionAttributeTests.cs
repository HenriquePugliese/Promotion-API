using Acropolis.Application.Features.Promotions;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionAttributeTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should create a promotion attribute with skus")]
    public void CreatePromotionAttribute_WithSkus_ShoudCreatePromotionAttribute()
    {
        //Arrange
        var promotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);
        var attributesId = "attrs-id1";
        var amount = 10;
        var skus = new List<PromotionAttributeSku>()
        {
            new PromotionAttributeSku("sku-1")
        };

        //Act
        var newPromotionAttribute = new PromotionAttribute(attributesId, amount, skus, promotion);

        //Assert
        newPromotionAttribute.SKUs.Should().NotBeNull();
        newPromotionAttribute.SKUs.Should().HaveCount(skus.Count);
        newPromotionAttribute.PromotionId.Should().Be(promotion.Id);
    }

    [Fact(DisplayName = "Should create a promotion attribute without skus")]
    public void CreatePromotionAttribute_WithoutSkus_ShoudCreatePromotionAttribute()
    {
        //Arrange
        var attributesId = "attrs-id1";
        var amount = 5;

        //Act
        var newPromotionAttribute = new PromotionAttribute(attributesId, amount, null);

        //Assert
        newPromotionAttribute.SKUs.Should().BeEmpty();
    }

}