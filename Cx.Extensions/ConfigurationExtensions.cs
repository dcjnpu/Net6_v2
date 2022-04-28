using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace Cx.Extensions
{
    /// <summary>
    /// IConfigurationSection 扩展操作
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// 带默认参数的配置获取
        /// </summary>
        /// <param name="configurationSection"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static string ValueWithDefault(this IConfigurationSection configurationSection, string defaultvalue = "")
        {
            var value = configurationSection.Value;
            return string.IsNullOrEmpty(value) ? defaultvalue : value;
        }
    }
}