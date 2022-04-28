using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cx.SqlSugarV2
{
    /// <summary>
    /// 店铺活动方式
    /// </summary>
    public enum DbOrderEnum
    {
        /// <summary>
        /// 打折
        /// </summary>
        [Description("排序Asc")]
        Asc = 1,

        /// <summary>
        /// 满减
        /// </summary>
        [Description("排序Desc")]
        Desc = 2
    }

    public class Page<T>
    {
        /// <summary>
        /// 当前页索引
        /// </summary>
        public long CurrentPage { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalItems { get; set; }
        /// <summary>
        /// 每页的记录数
        /// </summary>
        public long ItemsPerPage { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        public List<T> Items { get; set; }
    }


    public class ApiResult<T> where T : class
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; } = true;

        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int statusCode { get; set; } = 200;

        /// <summary>
        /// 数据集
        /// </summary>
        public T data { get; set; }
    }

    /// <summary>
    /// 查询count
    /// </summary>
    public class ResultCount
    {
        public int Count { get; set; } = 0;
    }

    /// <summary>
    /// 是否存在
    /// </summary>
    public class ResultAny
    {
        public bool Any { get; set; } = false;
    }

    /// <summary>
    /// 自定义排序
    /// </summary>
    public class ParmSort
    {
        public int p { get; set; }
        public int i { get; set; }
        public int o { get; set; }
    }

    /// <summary>
    /// 自定义排序
    /// </summary>
    public class ParmStringSort
    {
        public string p { get; set; }
        public string i { get; set; }
        public int o { get; set; }
    }

    /// <summary>
    /// 提供字符串FromBody的解析
    /// </summary>
    public class ParmString
    {
        public string parm { get; set; }
    }

    /// <summary>
    /// 提供字符串FromBody的解析
    /// </summary>
    public class ParmInt
    {
        public int id { get; set; }
    }

    /// <summary>
    /// 分页参数
    /// </summary>
    public class PageParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; } = 1;

        /// <summary>
        /// 每页总条数
        /// </summary>
        public int limit { get; set; } = 15;

        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; } = 0;

        /// <summary>
        /// 编号
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 类型条件
        /// </summary>
        public int types { get; set; } = 0;

        /// <summary>
        /// 审核状态
        /// </summary>
        public int audit { get; set; } = -1;

        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; } = -1;

        /// <summary>
        /// 属性
        /// </summary>
        public int attr { get; set; } = 0;

        /// <summary>
        /// 搜索日期，可能是2个日期，通过-分隔
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 排序方式，可根据数字来判断，
        /// </summary>
        public int orderType { get; set; } = 0;

        /// <summary>
        /// 排序的字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 排序的类型 asc  desc
        /// </summary>
        public string order { get; set; } = "desc";

        /// <summary>
        /// 动态条件
        /// </summary>
        public string where { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public string site { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string number { get; set; }
    }

    public class AppSearchParm
    {
        public string btime { get; set; }
        public string etime { get; set; }
        public string brand { get; set; }
        public int saleType { get; set; } = 0;
        public int activityTypes { get; set; } = 0;
    }

    public enum ApiEnum
    {
        /// <summary>
        /// 请求(或处理)成功
        /// </summary>
        [Description("请求(或处理)成功")]
        Status = 200, //请求(或处理)成功

        /// <summary>
        /// 内部请求出错
        /// </summary>
        [Description("内部请求出错")]
        Error = 500, //内部请求出错

        /// <summary>
        /// 未授权标识
        /// </summary>
        [Description("未授权标识")]
        Unauthorized = 401,//未授权标识

        /// <summary>
        /// 请求参数不完整或不正确
        /// </summary>
        [Description("请求参数不完整或不正确")]
        ParameterError = 400,//请求参数不完整或不正确

        /// <summary>
        /// 请求TOKEN失效
        /// </summary>
        [Description("请求TOKEN失效")]
        TokenInvalid = 403,//请求TOKEN失效

        /// <summary>
        /// HTTP请求类型不合法
        /// </summary>
        [Description("HTTP请求类型不合法")]
        HttpMehtodError = 405,//HTTP请求类型不合法

        /// <summary>
        /// HTTP请求不合法,请求参数可能被篡改
        /// </summary>
        [Description("HTTP请求不合法,请求参数可能被篡改")]
        HttpRequestError = 406,//HTTP请求不合法

        /// <summary>
        /// 该URL已经失效
        /// </summary>
        [Description("该URL已经失效")]
        URLExpireError = 407,//HTTP请求不合法

        /// <summary>
        /// 登录过期
        /// </summary>
        [Description("登录过期")]
        LoginExpireError = 408,//登录已过期

        #region 失败

        [Description("操作失败")]
        OprateFail = -201, //请求(或处理)成功

        [Description("没有数据")]
        DataNull = -202, //请求(或处理)成功

        #endregion 失败
    }

}
