using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Byohar.Application.Extensions;
public class JsonHelper
{
    public static string SerializeExpandoObject(ExpandoObject expando)
    {
        return JsonSerializer.Serialize(expando);
    }

    public static ExpandoObject DeserializeExpandoObject(string jsonData)
    {
        return JsonSerializer.Deserialize<ExpandoObject>(jsonData);
    }
}
