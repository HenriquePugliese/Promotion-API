using Acropolis.Application.Features.Promotions;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionAttributeSkuTests
{
    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    [Fact(DisplayName = "Should create active promotion attribute sku")]
    public void CreatePromotionAttributeSku_Active_ShoudCreatePromotionAttributeSku()
    {
        //Arrange
        var promotion = PromotionHelper.CreateValidPromotion(SellerId_Votorantim);
        var attributesId = "attrs-id1";
        var amount = 10;
        var skus = new List<PromotionAttributeSku>() { new PromotionAttributeSku("sku-1") };
        var promotionAttribute = new PromotionAttribute(attributesId, amount, skus, promotion);
        var skuValue = "skuval1";

        //Act
        var newPromotionAttributeSku = new PromotionAttributeSku(skuValue, promotionAttribute);

        //Assert
        newPromotionAttributeSku.Value.Should().NotBeNull();
        newPromotionAttributeSku.Value.Should().Be(skuValue);
        newPromotionAttributeSku.PromotionAttributeId.Should().Be(promotionAttribute.Id);
    }

}