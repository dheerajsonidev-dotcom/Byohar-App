using System.Linq.Expressions;

namespace Byohar.Application.Extensions;

public static class ExcelExportExtensions
{
    public static Dictionary<string, Func<T, object>> MapPropertiesToExcel<T>(this T source)
    {
        return typeof(T)
            .GetProperties()
            .ToDictionary(
                property => property.Name,
                property => (Func<T, object>)Delegate.CreateDelegate(
                    typeof(Func<T, object>),
                    source,
                    property.GetMethod));
    }
}
