using System.ComponentModel;

namespace Byohar.Application.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes?.Length > 0
                ? attributes[0].Description
                : val.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (Enum.TryParse(value, true, out TEnum result))
            {
                return result;
            }

            throw new ArgumentException($"Invalid enum value: {value}");
        }
    }
}