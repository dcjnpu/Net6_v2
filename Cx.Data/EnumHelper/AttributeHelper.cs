using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// 自定义属性 方法
    /// </summary>
    public class AttributeHelper
    {
        /// <summary>
        /// 检查自定义属性中有没有该属性
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ContainMyAttribute(PropertyInfo property,Type type)
        {
            var attr = property.GetCustomAttributes(type).FirstOrDefault();
            if (attr==null) return false;
            else return true;
        }
    }
}
