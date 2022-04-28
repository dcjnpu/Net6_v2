using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    public static class EnumDescriptionExtension
    {
        private static Dictionary<string, Dictionary<string, string>> enumCache = new Dictionary<string, Dictionary<string, string>>();

        private static Dictionary<string, Dictionary<string, string>> EnumCache
        {
            get => enumCache; set => enumCache = value;
        }

        private static Dictionary<string, Dictionary<int, string>> enumCache2 = new Dictionary<string, Dictionary<int, string>>();

        private static Dictionary<string, Dictionary<int, string>> EnumCache2
        {
            get => enumCache2; set => enumCache2 = value;
        }


        /// <summary>
        /// 获得枚举提示文本
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum en)
        {
            string enString = string.Empty;
            if (null == en) return enString;
            var type = en.GetType();
            enString = en.ToString();

            string fullname = type.FullName ?? "";
            if (string.IsNullOrWhiteSpace(fullname)) return enString;
            if (!EnumCache.ContainsKey(fullname))
            {
                InitEnum(type);
            }
            if (EnumCache[fullname].ContainsKey(enString))
            {
                return EnumCache[fullname][enString];
            }
            return enString;
        }

        /// <summary>
        /// 获得枚举提示文本
        /// </summary>
        /// <param name="type"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Type type, int en)
        {
            string enString = en.ToString();

            string fullname = type.FullName ?? "";
            if (string.IsNullOrWhiteSpace(fullname)) return enString;
            if (!EnumCache2.ContainsKey(fullname))
            {
                InitEnum(type);
            }
            if (EnumCache2[fullname].ContainsKey(en))
            {
                return EnumCache2[fullname][en];
            }
            return enString;
        }

        private static void InitEnum(Type type)
        {
            string fullname = type.FullName ?? "";
            var fields = type.GetFields();
            Dictionary<string, string> temp = new Dictionary<string, string>();
            Dictionary<int, string> temp2 = new Dictionary<int, string>();
            foreach (var item in fields)
            {
                var attrs = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length == 1)
                {
                    var v = ((DescriptionAttribute)attrs[0]).Description;
                    temp.Add(item.Name, v);
                    temp2.Add(item.GetRawConstantValue()?.ToInt32() ?? 0, v);
                }
            }
            EnumCache.Add(fullname, temp);
            EnumCache2.Add(fullname, temp2);
        }

        public static List<TKeyValue> GetFields(Type type)
        {
            List<TKeyValue> list = new List<TKeyValue>();
            string fullname = type.FullName ?? "";
            if (string.IsNullOrWhiteSpace(fullname)) return list;
            if (!EnumCache.ContainsKey(fullname))
            {
                InitEnum(type);
            }
            Dictionary<string, string> alist = EnumCache[fullname];
            foreach (var item in alist) list.Add(new TKeyValue() { Key = item.Key, Value = item.Value });
            return list;
        }

        public static List<TKeyValue> GetFields_Int(Type type)
        {
            List<TKeyValue> list = new List<TKeyValue>();
            string fullname = type.FullName ?? "";
            if (string.IsNullOrWhiteSpace(fullname)) return list;
            if (!EnumCache2.ContainsKey(fullname))
            {
                InitEnum(type);
            }
            Dictionary<int, string> alist = EnumCache2[fullname];
            foreach (var item in alist) list.Add(new TKeyValue() { Key = item.Key.ToString(), Value = item.Value });
            return list;
        }
    }

    ///// <summary>
    ///// 保存的枚举信息
    ///// </summary>
    //public class T_Enum_Info
    //{
    //    /// <summary>
    //    /// 枚举的名字，英文部分
    //    /// </summary>
    //    public string name { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 枚举的说明 ，attrbute部分
    //    /// </summary>
    //    public string description { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 枚举的值，int部分
    //    /// </summary>
    //    public int value { get; set; }
    //}
}