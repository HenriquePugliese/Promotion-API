namespace Acropolis.Application.Base.Extensions
{
    public static class StringExtensions
    {
        public static string OnlyNumbers(this string text)
        {
            return string.Concat($"{text}".Where(c => char.IsDigit(c)));
        }

        public static Guid ToGuid(this string text) => new(text);        
    }
}
