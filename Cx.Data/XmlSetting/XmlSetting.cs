using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// Setting.Config读取,新版不再使用
    /// </summary>
    public class XmlSetting
    {
        private string _path = string.Empty;
        private string _cachename = string.Empty;



        XmlSetting(string path)
        {
            _path = path;
            _cachename = "config_setting";
        }

        public static XmlSetting Default { get; private set; }
        static XmlSetting()
        {
            Default = new XmlSetting("config/setting.config");
            Default.Init();
        }


        void Init()
        {
            var data = XmlSettingReader.Reader2(_path).Result;
            if (data == null) return;
            foreach (string key in data.Keys)
            {
                MemoryCacheService.Default.SetCache(_cachename + "_" + key, data[key]);
            }
        }

        public string Get(string key)
        {
            return MemoryCacheService.Default.GetCache<string>(_cachename + "_" + key);
        }


    }
}
