namespace Acropolis.Application.Base.Converters
{
    public static class WeightConverter
    {
        public static decimal ToTon(decimal kiloValue, int decimals = 4) => decimal.Round(kiloValue / 1000, decimals);
        public static decimal ToKilo(decimal tonValue, int decimals = 4) => decimal.Round(tonValue * 1000, decimals);
    }
}