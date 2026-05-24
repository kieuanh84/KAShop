using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace TrangWebBanHang.Helpers
{
    public static class SessionExtensions
    {
        // Hàm lưu Object vào Session (Chuyển Object thành chuỗi Json)
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Hàm lấy Object từ Session (Chuyển chuỗi Json ngược lại thành Object)
        public static T? GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}