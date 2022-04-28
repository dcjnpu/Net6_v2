using Cx.SqlSugarV2;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaseBLL
{
    public class AdminHelper
    {
        public static bool HasPower(int id, string power)
        {
            if (power.Length < id) return false;
            return power.Substring(id - 1, 1).ToInt32() == 1;
        }
    }

    public class AdminConst
    {
        public const string Http_AdminMenu = "Http_AdminMenu";
        public const string Http_Role = "Http_Role";
        public const string Http_SiteInfo = "Http_SiteInfo";
    }

    public class PageHelper
    {
        /// <summary>
        /// 将系统自带的ApiResult转为正常的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static TPage_Layer<T> GetPage<T>(ApiResult<Page<T>> result)
        {
            return new TPage_Layer<T>()
            {
                code = result.statusCode == 200 ? 0 : result.statusCode,
                count = result.data?.TotalItems ?? 0,
                data = result.data?.Items,
                message = result.message
            };
        }

        public static TPage_Layer<T> GetPage<T>(Page<T> result)
        {
            return new TPage_Layer<T>()
            {
                code = result != null ? 0 : 400,
                count = result?.TotalItems ?? 0,
                data = result?.Items,
                message = result != null ? "获取成功" : "失败",
            };
        }
    }

    public class TPage_Layer<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public long count { get; set; }
        public List<T>? data { get; set; }
    }

    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}