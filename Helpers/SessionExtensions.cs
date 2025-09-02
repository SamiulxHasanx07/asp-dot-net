using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Helpers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
