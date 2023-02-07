using Acropolis.Application.Features.Promotions;
using Acropolis.Tests.Helpers.Api;
using FluentAssertions;

namespace Acropolis.Tests.Application.Features.Promotions;

public class PromotionCnpjTests
{

    [Fact(DisplayName = "Should create valid promotion cnpj")]
    public void CreatePromotionCnpj_WithValidPromotion_ShoudCreatePromotionCnpj()
    {
        //Arrange
        var sellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");
        var externalId = "andslfnds-111a";
        var cnpj = "170983111000173";
        var promotion = PromotionHelper.CreateValidPromotion(sellerId, externalId);

        //Act
        var newPromotionCnpj = new PromotionCnpj(externalId, cnpj, sellerId, promotion);

        //Assert
        newPromotionCnpj.ExternalId.Should().Be(externalId);
        newPromotionCnpj.Cnpj.Should().Be(cnpj);
        newPromotionCnpj.PromotionId.Should().Be(promotion.Id);
    }

}