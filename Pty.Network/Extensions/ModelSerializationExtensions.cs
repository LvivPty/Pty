using Newtonsoft.Json;
using Pty.Network.Models;
using Pty.Network.Models.Commands;

namespace Pty.Network.Extensions
{
    public static class ModelSerializationExtensions
    {
        public static TModel Deserialize<TModel>(this string serialized)
        {
            return JsonConvert.DeserializeObject<TModel>(serialized);
        }

        public static string Serialize<T>(this T model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
