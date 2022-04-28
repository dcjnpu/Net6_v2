using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cx.Data
{
    /// <summary>
    /// 从config/setting.json文件从读取配置
    /// </summary>
    public class ConfigSetting
    {
        public static IConfiguration Data { get; private set; }
        static ConfigSetting()
        {
            Data = ConfigExtensions.GetConfig("config/setting.json");
        }
    }
}
