using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Features.Attributes.Enums;
using Acropolis.Application.Features.Promotions.Enums;
using FluentAssertions;

namespace Acropolis.Tests.Application.Base.Extensions
{
    public class AttributeExtensionsTests
    {
        [Fact(DisplayName = "Should get description from enum when attribute exists")]
        public void GetDescription_EnumWithAttributeDescription_ShouldReturnValidDescription()
        {
            // arrange
            var kilogram = WeightType.Kilogram;

            // act
            var kilogramDescription = kilogram.GetDescription();

            // assert
            kilogramDescription.Should().Be("kg");
        }

        [Fact(DisplayName = "Should get empty description from enum when attribute not exists")]
        public void GetDescription_EnumWithoutAttributeDescription_ShouldReturnEmptyDescription()
        {
            // arrange
            var state = PromotionParameter.UF;

            // act
            var stateDescription = state.GetDescription();

            // assert
            stateDescription.Should().BeEmpty();
        }
    }
}
