using System.Collections;
using System.Data;
using System.Text;

namespace Cx.Data
{
    /// <summary>
    /// 初始化后的不动变量
    /// </summary>
    public class Base
    {
       


        /// <summary>
        /// 时间戳
        /// </summary>
        public static string TimeSpan_Second_8
        {
            get
            {
                DateTime time_1970 = Convert.ToDateTime("1970-01-01 00:00:00");
                TimeSpan ts = DateTime.Now - time_1970;
                return Convert.ToInt32(ts.TotalSeconds).ToString();
            }
        }

        /// <summary>
        /// 时间戳 ,1970-01-01 08
        /// </summary>
        public static string TimeSpan_Second_0
        {
            get
            {
                DateTime time_1970 = Convert.ToDateTime("1970-01-01 08:00:00");
                TimeSpan ts = DateTime.Now - time_1970;
                return Convert.ToInt32(ts.TotalSeconds).ToString();
            }
        }
        #region 随机数
        /// <summary>
        /// 获取一组随机数
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="MaxLen"></param>
        /// <returns></returns>
        public static string GetRandom(int rt, int MaxLen)
        {
            Random ran = null; int seed = GetTickNum(9); string result = string.Empty; List<int> rt_s = null;
            rt_s = new List<int>();
            if ((rt & (int)RandType.Num) == (int)RandType.Num) rt_s.Add((int)RandType.Num);
            if ((rt & (int)RandType.Character) == (int)RandType.Character) rt_s.Add((int)RandType.Character);
            if ((rt & (int)RandType.ZH) == (int)RandType.ZH) rt_s.Add((int)RandType.ZH);

            int rts_len = rt_s.Count; int rt_index = 0; int rt_i = rt;
            for (int i = 0; i < MaxLen; i++)
            {
                if (rts_len > 1)
                {
                    ran = new Random(seed + i * 1000);
                    rt_index = ran.Next(0, rts_len);
                    rt_i = rt_s[rt_index];
                }

                switch (rt_i)
                {
                    case (int)RandType.Num: result += GetRandom_Num(seed + i * 2000); break;
                    case (int)RandType.Character: result += GetRandom_C(seed + i * 3000); break;
                    case (int)RandType.ZH: result += GetRandom_ZH(seed + i * 4000); break;
                    default: result += GetRandom_Num(seed + i * 2000); break;
                }
            }
            rt_s = null;
            return result;
        }




        private static int GetTickNum(int MaxLen)
        {
            return Convert.ToInt32((GetTickStr(MaxLen)));
        }

        private static string GetTickStr(int MaxLen)
        {
            if (MaxLen > 10) MaxLen = 10;
            string a = DateTime.Now.Ticks.ToString();
            return a.Substring(a.Length - MaxLen, MaxLen);
        }

        private static string GetRandom_Num(int seed)
        {
            Random r = new Random(seed);
            return r.Next(0, 10).ToString();
        }

        private static string GetRandom_C(int seed)
        {
            Random r = new Random(seed);
            int assNum_10 = r.Next(97, 123);
            return ((char)assNum_10).ToString();
        }


        /// <summary>
        /// 获取随机产生的中文
        /// </summary>
        /// <param name="Encode">产生随机中文的编码</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        private static string GetRandom_ZH(int seed)
        {
            //定义返回的字符串
            string Chinese = "";
            //定义中文编码
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Encoding Ecode = Encoding.GetEncoding("GB2312");
            Random Rnd = null;// new Random(GetIntRand(4));
                              //int Rint = Rnd.Next(1, 100);
                              //定义位码、区码的范围数
            int Wint, Qint;

            int Rint = 0;
            //获取汉字区位
            Rnd = new Random(seed);
            Rint = Rnd.Next(16, 56);//只获取常用汉字 16-55之间
            Wint = Rint;
            //55区只89个汉字 其他 94个汉字
            Rint = (Wint == 55) ? 90 : 95;
            //定义新种子
            Rnd = new Random(seed + 1000);
            Rint = Rnd.Next(1, Rint);
            Qint = Rint;
            //两个字节变量存储产生的随机汉字区位码
            byte Fbyte = System.Convert.ToByte((Wint + 160).ToString("x"), 16);
            byte Lbyte = System.Convert.ToByte((Qint + 160).ToString("x"), 16);
            //将两个字节变量存储在字节数组中
            byte[] Nbytes = new byte[] { Fbyte, Lbyte };
            Chinese = Ecode.GetString(Nbytes);
            Rnd = null;
            Ecode = null;
            return Chinese;
        }

        #endregion
        /// <summary>
        /// 返回WebConfig里的APPKEY的value
        /// 新版从appsettings.json中获取
        public static string GetAppKeyValue(string Key)
        {
            return ConfigExtensions.AppSettings.GetSection(Key).Value;
        }

        /// <summary>
        /// 返回WebConfig里的APPKEY的value
        /// 新版从appsettings.json中获取
        /// </summary>
        /// <param name="Key">Web.Config 中指定的 Key</param>
        /// <param name="Value">若Web.Config 中无指定的值 则显示此默认值</param>
        /// <returns>Value</returns>
        public static string GetAppKeyValue(string Key, string Value)
        {
            var KeyVals = ConfigExtensions.AppSettings.GetSection(Key).Value;
            //string KeyVals = ConfigurationManager.AppSettings[Key];
            if (string.IsNullOrEmpty(KeyVals)) KeyVals = Value;
            return KeyVals;
        }
        /// <summary>
        /// 返回setting.json里的APPKEY的value
        /// </summary>
        public static string GetSettingKeyValue(string Key)
        {
            return ConfigSetting.Data.GetSection(Key).Value;
        }
        /// <summary>
        /// 返回setting.json里的APPKEY的value
        /// </summary>
        /// <param name="Key">Web.Config 中指定的 Key</param>
        /// <param name="Value">若Web.Config 中无指定的值 则显示此默认值</param>
        /// <returns>Value</returns>
        public static string GetSettingKeyValue(string Key, string Value)
        {
            var KeyVals = ConfigSetting.Data.GetSection(Key).Value;
            //string KeyVals = ConfigurationManager.AppSettings[Key];
            if (string.IsNullOrEmpty(KeyVals)) KeyVals = Value;
            return KeyVals;
        }

        /// <summary>
        /// 转化为hashtable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Hashtable SetFirstRowToHt(DataTable dt)
        {
            Hashtable ht = new Hashtable();
            if (dt == null || dt.Rows.Count == 0) return ht;
            foreach (DataColumn dc in dt.Columns)
            {
                ht.Add(dc.ColumnName, dt.Rows[0][dc.ColumnName]);
            }
            return ht;
        }

        #region 两种查询datatable的方法
        public static DataTable GetTable(DataTable dt, string where)
        {
            DataTable dt2 = dt.Clone();
            DataRow[] dr = dt.Select(where);
            for (int i = 0; i < dr.Length; i++)
            {
                dt2.ImportRow((DataRow)dr[i]);
            }
            return dt2;
        }

        public static DataTable GetTable2(DataTable dt, string where)
        {
            DataTable dt2 = dt.Clone();
            DataRow[] dr = dt.Select(where);
            for (int i = 0; i < dr.Length; i++)
            {
                dt2.Rows.Add(dr[i].ItemArray);
            }
            return dt2;
        }
        #endregion


    }

    public class Compare<T, C> : IEqualityComparer<T>
    {
        private Func<T, C> _getField;
        public Compare(Func<T, C> getfield)
        {
            this._getField = getfield;
        }
        public bool Equals(T? x, T? y)
        {
            if (x == null && y == null) return true;
            if (x != null && y != null)
                return EqualityComparer<C>.Default.Equals(_getField(x), _getField(y));
            return false;
        }
        public int GetHashCode(T obj)
        {
            if (obj == null) return 0;
            return EqualityComparer<C>.Default.GetHashCode(_getField(obj));
        }
    }
    public static class CommonHelper
    {
        /// <summary>
        /// 自定义Distinct扩展方法
        /// </summary>
        /// <typeparam name="T">要去重的对象类</typeparam>
        /// <typeparam name="C">自定义去重的字段类型</typeparam>
        /// <param name="source">要去重的对象</param>
        /// <param name="getfield">获取自定义去重字段的委托</param>
        /// <returns></returns>
        public static IEnumerable<T> CurDistinct<T, C>(this IEnumerable<T> source, Func<T, C> getfield)
        {
            return source.Distinct(new Compare<T, C>(getfield));
        }
    }

    /// <summary>
    /// 随机类型
    /// </summary>
    public enum RandType
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Text("数字")]
        Num = 1,
        [Text("字母")]
        Character = 2,
        [Text("汉字")]
        ZH = 4,
    }

    /// <summary>
    /// 存储过程操作
    /// </summary>
    public enum EProcAction
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Text("新增")]
        Add = 0,
        /// <summary>
        /// 编辑
        /// </summary>
        [Text("编辑")]
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        [Text("删除")]
        Del = -1
    }
}
