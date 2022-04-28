using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cx.Data
{
    public class ConfigExtensions
    {
        public static IConfiguration AppSettings { get; set; }
        static ConfigExtensions()
        {
            AppSettings = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }
        /// <summary>
        /// 获得配置文件的对象值
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetJson(string jsonPath, string key)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build(); //json文件地址
            string s = config.GetSection(key).Value; //json某个对象
            return s;
        }
        public static IConfiguration GetConfig(string jsonPath)
        {
            //IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build(); //json文件地址
            return new ConfigurationBuilder().AddJsonFile(jsonPath, true, true).Build();
            //string s = config.GetSection(key).Value; //json某个对象
            //return s;
        }
        /// <summary>
        /// 根据配置文件和Key获得对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">文件名称</param>
        /// <param name="key">节点Key</param>
        /// <returns></returns>
        //public static T GetAppSettings<T>(string fileName, string key) where T : class, new()
        //{
        //    var baseDir = AppContext.BaseDirectory + "json/";
        //    var currentClassDir = baseDir;

        //    IConfiguration config = new ConfigurationBuilder()
        //        .SetBasePath(currentClassDir)
        //        .Add(new JsonConfigurationSource { Path = fileName, ReloadOnChange = true })
        //        .Build();

        //    var appconfig = new ServiceCollection().AddOptions()
        //       //.Configure<T>(config.GetSection(key))
        //       .Configure<T>(h=>{h. })
        //        .BuildServiceProvider()
        //        .GetService<IOptions<T>>()
        //        .Value;
        //    return appconfig;
        //}
    }
}
