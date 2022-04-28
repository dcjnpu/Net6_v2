using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    public static class EnumTextExtension
    {
        private static Dictionary<string, Dictionary<string, string>> enumCache = new Dictionary<string, Dictionary<string, string>>();

        private static Dictionary<string, Dictionary<string, string>> EnumCache
        {
            get => enumCache; set => enumCache = value;
            //get
            //{
            //    return enumCache;
            //}
            //set { enumCache = value; }
        }

        /// <summary>
        /// 获得枚举提示文本
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumText(this Enum en)
        {
            string enString = string.Empty;
            if (null == en) return enString;
            var type = en.GetType();
            enString = en.ToString();

            string fullname = type.FullName ?? "";
            if (string.IsNullOrWhiteSpace(fullname)) return enString;
            if (!EnumCache.ContainsKey(fullname))
            {
                var fields = type.GetFields();
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (var item in fields)
                {
                    var attrs = item.GetCustomAttributes(typeof(TextAttribute), false);
                    if (attrs.Length == 1)
                    {
                        var v = ((TextAttribute)attrs[0]).Value;
                        temp.Add(item.Name, v);
                    }
                }
                EnumCache.Add(fullname, temp);
            }
            if (EnumCache[fullname].ContainsKey(enString))
            {
                return EnumCache[fullname][enString];
            }
            return enString;
        }
    }

    public class TextAttribute : Attribute
    {
        public TextAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}