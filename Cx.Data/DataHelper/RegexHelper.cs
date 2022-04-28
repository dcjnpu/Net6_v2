using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cx.Data
{
    /// <summary>
    /// 通用正则方法
    /// </summary>
    public class RegexHelper
    {
        /// <summary>
        /// 获取>---<中的内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetNote(string html)
        {
            Regex r_Src = new Regex(">.+?<", RegexOptions.Singleline);
            if (!r_Src.IsMatch(html))
                return "";
            return r_Src.Match(html).ToString().Replace("<", "").Replace(">", "");
        }
        /// <summary>
        /// 获取字符串的字节长度
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int GetStringLenZh(string Source)
        {
            if (string.IsNullOrEmpty(Source)) return 0;

            Regex R = new Regex("[\u4e00-\u9fa5]", RegexOptions.None);
            int Zh_Count = R.Matches(Source).Count;
            return Zh_Count * 2 + (Source.Length - Zh_Count);
        }

        /// <summary>
        /// 判断是否为中文
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static bool IsZH(string A)
        {
            Regex R = new Regex("[\u4e00-\u9fa5]", RegexOptions.None);
            return R.IsMatch(A);
        }

        /// <summary>
        /// 取出中间空格
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static string ClearSpace(string A)
        {
            Regex R = new Regex("\\s", RegexOptions.None);
            return R.Replace(A, "");
            //return  result.replace(/\s/g,"");//去除文章中间空格
        }
        /// <summary>
        /// 去除换行 
        /// </summary>
        /// <returns></returns>
        public static string ClearEnter(string source)
        {
            Regex r = new Regex("[\\r\\n]", RegexOptions.None);
            return r.Replace(source, "");
        }
        /// <summary>
        /// 获取纯数字字符串
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static string GetNumberStr(string A)
        {
            Regex R = new Regex(@"[^0-9|\\.]", RegexOptions.None);
            return R.Replace(A, "").ToString();
        }


        public static bool IsNumber(string A)
        {
            if (string.IsNullOrEmpty(A))
                return false;

            Regex R = new Regex(@"^\d{1,}$", RegexOptions.None);
            return R.IsMatch(A);
        }

        public static bool IsIDCard(string A)
        {
            string b = string.Empty;
            bool flag = IsIDCard(A, ref b);
            if (!flag) return false;
            b = b.Substring(0, 4) + "-" + b.Substring(4, 2) + "-" + b.Substring(6, 2);
            try
            {
                System.DateTime birthday = Convert.ToDateTime(b);
                if (birthday == System.DateTime.MinValue) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsIDCard(string A, ref System.DateTime birthday, ref bool IsBoy)
        {
            string b = string.Empty;
            bool flag = IsIDCard(A, ref b);
            if (!flag) return false;

            b = b.Substring(0, 4) + "-" + b.Substring(4, 2) + "-" + b.Substring(6, 2);

            try
            {
                birthday = Convert.ToDateTime(b);
                if (birthday == System.DateTime.MinValue) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsIDCard(string A, ref string birthday)
        {
            if (string.IsNullOrEmpty(A)) return false;
            if (A.Length == 18)
            {
                Regex R = new Regex(@"^[1-9]\d{5}[1-2]\d{3}[0-1]\d[0-3]\d{4}(\d|X|x)$", RegexOptions.None);

                birthday = A.Substring(6, 8);
                //IsBoy = CxConvert.ToInt(A.Substring(16, 1)) % 2 == 0 ? false : true;
                return R.IsMatch(A);
            }
            if (A.Length == 15)
            {
                Regex R = new Regex(@"^[1-9]\d{7}[0-1]\d[0-3]\d{3}(\d|X)$", RegexOptions.None);
                birthday = "19" + A.Substring(6, 6);
                return R.IsMatch(A);
            }
            return false;
        }

    }
}
