using CSRedis;
using SqlSugar;
using System.ComponentModel;

namespace Cx.Redis
{
    [SugarTable("PT_CACHE")]
    public class T_Cache_Redis
    {
        /// <summary>
        /// 缓存关键字
        /// </summary>
        public string CKey = string.Empty;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName = string.Empty;
        /// <summary>
        /// 名称
        /// </summary>
        public string Conn = string.Empty;
        /// <summary>
        /// 查询语句
        /// </summary>
        public string JustSql = "";
        /// <summary>
        /// 自动刷新时间
        /// </summary>
        public int RefreshSecond = 0;
        /// <summary>
        /// 名称
        /// </summary>
        public string CTitle = string.Empty;

        /// <summary>
        /// E_CacheDataType 0：hashtable，1：DataTable
        /// </summary>
        public int DataType = 0;

        /// <summary>
        /// 如果是hashtable ht的key
        /// </summary>
        public string ShowKey = string.Empty;

        /// <summary>
        /// 如果是hashtable ht的value
        /// </summary>
        public string ShowValue = string.Empty;

    }


    /// <summary>
    /// 一般缓存
    /// </summary>
    public enum E_CacheDataType
    {
        [Description("Hashtable")]
        hashtable = 0,
        [Description("DataTable")]
        datatable = 1,
        [Description("String")]
        character = 2
    }
}