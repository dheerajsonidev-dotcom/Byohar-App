using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Byohar.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static T ConvertUtcDateTimePropertiesToLocalTime<T>(this T obj, HttpContext httpContext)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            
            return obj;
        }

       

    }
}