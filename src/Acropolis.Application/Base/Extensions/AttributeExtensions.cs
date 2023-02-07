using System.ComponentModel;
using System.Reflection;

namespace Acropolis.Application.Base.Extensions
{
    public static class AttributeExtensions
    {
        public static string GetDescription(this Enum enumValue) => $"{enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DescriptionAttribute>()?.Description}";
    }
}