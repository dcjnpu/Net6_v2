using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// 显示名 设定属性
    /// </summary>
    public class ShowNameAttrbute : Attribute
    {
        private string ColumnName { get; set; }
        /// <summary>
        /// 设定名称
        /// </summary>
        /// <param name="key">显示名</param>
        public ShowNameAttrbute(string key)
        {
            ColumnName = key;
        }

        /// <summary>
        /// 获取该attribute的ShowName
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetShowName(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(ShowNameAttrbute)).FirstOrDefault();
            if (attr == null) return String.Empty;
            return ((ShowNameAttrbute)attr).ColumnName;
        }

        /// <summary>
        /// 检查是否含有该Attribute
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool ContainMyAttribute(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(ShowNameAttrbute)).FirstOrDefault();
            if (attr==null) return false;
            else return true;
        }

        /// <summary>
        /// 获取该Class的所有可读public实例
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetColumns(Type T)
        {
            var properties = T.GetProperties(BindingFlags.Public|BindingFlags.Instance).Where(h => h.CanRead&&ContainMyAttribute(h)).ToArray();
            return properties;
        }
    }
}
