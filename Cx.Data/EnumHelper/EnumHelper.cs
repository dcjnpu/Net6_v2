using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// 绑定枚举
    /// </summary>
    public class BindEnum
    {


        #region layui 用的绑定checkbox和radio
        private static string EnumConvert(Type t, int en)
        {
            return EnumDescriptionExtension.GetEnumDescription(t, en);
        }

        /// <summary>
        /// 绑定枚举到指定字符串
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="stringFormats"></param>
        /// <param name="Name"></param>
        /// <param name="Selected"></param>
        /// <returns></returns>
        public static string BindCheckBox_Layer(Type eType, string Name, int Selected, string stringFormats = "")
        {
            return BindCheckBox_Layer(eType, EnumConvert, Name, Selected, stringFormats);
        }
        /// <summary>
        /// 绑定枚举到指定字符串
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="Convert"></param>
        /// <param name="stringFormats"></param>
        /// <param name="Name"></param>
        /// <param name="Selected"></param>
        /// <returns></returns>
        public static string BindCheckBox_Layer(Type eType, Func<Type, int, string> Convert, string Name, int Selected, string stringFormats = "")
        {
            if (string.IsNullOrEmpty(stringFormats)) stringFormats = "<input type=\"checkbox\" lay-filter=\"{0}\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\" title=\"{2}\" {3}/>";
            StringBuilder sb = new StringBuilder();
            int i = 0; string f = string.Empty;
            foreach (var v in eType.GetFields())
            {
                if (v.FieldType.IsEnum)
                {
                    i = (v.GetRawConstantValue()).ToInt32();
                    f = Convert(v.FieldType, i);
                    if ((i & Selected) != i) sb.AppendFormat(stringFormats, Name, i.ToString(), f, "");
                    else sb.AppendFormat(stringFormats, Name, i.ToString(), f, "checked=\"checked\"");
                }

            }
            return sb.ToString();
        }
        /// <summary>
        /// 绑定枚举到指定字符串
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="Name"></param>
        /// <param name="Selected"></param>
        /// <param name="stringFormats"></param>
        /// <returns></returns>
        public static string BindRadio_Layer(Type eType, string Name, int Selected, string stringFormats = "")
        {
            return BindRadio_Layer(eType, EnumConvert, Name, Selected, stringFormats);
        }
        /// <summary>
        /// 绑定枚举到指定字符串
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="Convert"></param>
        /// <param name="Name"></param>
        /// <param name="Selected"></param>
        /// <param name="stringFormats"></param>
        /// <returns></returns>
        public static string BindRadio_Layer(Type eType, Func<Type, int, string> Convert, string Name, int Selected, string stringFormats = "")
        {
            string format = "<input type=\"radio\" lay-filter=\"{0}\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\"  title=\"{2}\" {3}/>";
            if (string.IsNullOrEmpty(stringFormats)) stringFormats = format;
            StringBuilder sb = new StringBuilder();
            int i = 0; string f = string.Empty;
            foreach (var v in eType.GetFields())
            {
                if (v.FieldType.IsEnum)
                {
                    i = (v.GetRawConstantValue()).ToInt32();
                    f = Convert(v.FieldType, i);
                    if (i != Selected) sb.AppendFormat(stringFormats, Name, i.ToString(), f, "");
                    else sb.AppendFormat(stringFormats, Name, i.ToString(), f, "checked=\"checked\"");
                }

            }
            return sb.ToString();
        }



        #endregion
    }
}
