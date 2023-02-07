using Acropolis.Application.Base.Converters;
using FluentAssertions;

namespace Acropolis.Tests.Application.Base.Converters
{
    public class WeightConverterTests
    {
        [Fact(DisplayName = "Should convert ton to kilo value")]
        public void Convert_WeightTonToKilo_ShouldConvertTonToKiloValue()
        {
            // arrange
            var tonValue = 10;

            // act
            var kiloValue = WeightConverter.ToKilo(tonValue);

            // assert
            kiloValue.Should().Be(10000);
        }

        [Fact(DisplayName = "Should convert kilo to ton value")]
        public void Convert_WeightKiloToTon_ShouldConvertKiloToTonValue()
        {
            // arrange
            var kiloValue = 500;

            // act
            var tonValue = WeightConverter.ToTon(kiloValue);

            // assert
            tonValue.Should().Be(0.5m);
        }

        [Fact(DisplayName = "Should convert kilo to ton value with especific decimals")]        
        public void Convert_WeightKiloToTonWithEspecificDecimals_ShouldConvertKiloToTonValue()
        {
            // arrange
            var kiloValue = 1;
            var especificDecimals = 3;

            // act
            var tonValue = WeightConverter.ToTon(kiloValue, especificDecimals);

            // assert
            tonValue.Should().Be(0.001m);
        }
    }
}
