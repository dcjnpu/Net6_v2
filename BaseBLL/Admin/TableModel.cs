using Cx.Jwt;
using SqlSugar;
using System.ComponentModel;

namespace BaseBLL
{
    #region 基础库表

    #region 基础库

    public interface I_TB_Base
    {
        int ID { get; set; }
    }

    public interface I_TB_Split_Base
    {
        long ID { get; set; }
    }

    public class PT_BASE
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
    }

    #endregion 基础库

    #region admin

    [SugarTable("PT_ADMIN")]
    public class PT_ADMIN : PT_BASE, I_TB_Base
    {
        [JwtKey]
        [SugarColumn(Length = 50, IsNullable = false)]
        public string UNAME { get; set; }
        [JwtKey]
        [SugarColumn(Length = 50)]
        public string REALNAME { get; set; }

        [SugarColumn(Length = 50, IsNullable = true)]
        public string PWD { get; set; }
        [JwtKey]
        public int ROLEID { get; set; }

        /// <summary>
        /// E_USTATE
        /// </summary>
        public int USTATE { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CTIME { get; set; }
    }

    public enum E_USTATE
    {
        [Description("正常")]
        Normal = 0,

        [Description("禁用")]
        NoLogin = 1,

        [Description("离职")]
        Leave = 2,
    }

    #endregion admin

    #region cache

    [SugarTable("PT_CACHE")]
    public class PT_CACHE : PT_BASE, I_TB_Base
    {
        [SugarColumn(Length = 50, IsNullable = false)]
        public string CKey { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string CTitle { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Conn { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string TableName { get; set; }

        [SugarColumn(Length = 500, IsNullable = false)]
        public string JustSql { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "-1")]
        public int RefreshSecond { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, DefaultValue = "")]
        public string ShowKey { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, DefaultValue = "")]
        public string ShowValue { get; set; }

        /// <summary>
        /// E_CacheDataType
        /// </summary>
        [SugarColumn(IsNullable = true, DefaultValue = "0")]
        public int DataType { get => dataType; set => dataType = value; }

        private int dataType = 0;

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CTIME { get; set; }
    }

    public enum E_CacheDataType
    {
        [Description("Hashtable")]
        hashtable = 0,

        [Description("DataTable")]
        datatable = 1,

        [Description("String")]
        character = 2
    }

    #endregion cache

    #region PT_LOG

    [SplitTable(SplitType.Month)]
    [SugarTable("PT_LOG_{year}{month}{day}")]
    public class PT_LOG : I_TB_Split_Base
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string OPID { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string OPNAME { get; set; }

        [SplitField]
        public DateTime OPTIME { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string OPIP { get; set; }

        /// <summary>
        /// E_AminLog
        /// </summary>
        public int OPTYPE { get; set; }

        [SugarColumn(Length = 4000, IsNullable = false)]
        public string NOTE { get; set; }
    }

    public enum E_AminLog
    {
        [Description("系统操作")]
        system = 0,

        [Description("登录/登出")]
        login = 1,

        [Description("编辑员工信息")]
        editadmin = 2,

        [Description("修改配置信息")]
        tree = 3,

        [Description("系统缓存配置")]
        systemcacheset = 4,

        [Description("后台角色")]
        adminrole = 5,

        [Description("站点信息")]
        siteinfo = 6,

        [Description("修改登录密码")]
        editadminpwd = 7,

        [Description("修改员工角色")]
        editadminrole = 8,

        [Description("修改菜单")]
        adminmenu = 9,

        #region 具体

        [Description("系统配置")]
        setting = 100,

        #endregion 具体
    }

    #endregion PT_LOG

    [SqlSugar.SugarTable("PT_ROLE")]
    public class PT_ROLE : PT_BASE, I_TB_Base
    {
        [SugarColumn(Length = 50, IsNullable = false)]
        public string NAME { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string EXPLAIN { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "Varchar(8000)", DefaultValue = "")]
        public string POWERS { get; set; }

        [SugarColumn(IsNullable = true, DefaultValue = "0")]
        public int LEVEL { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CTIME { get; set; }
    }

    [SqlSugar.SugarTable("PT_SITEINFO")]
    public class PT_SITEINFO : PT_BASE, I_TB_Base
    {
        [SugarColumn(Length = 50, IsNullable = false)]
        public string NAME { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string EXPLAIN { get; set; }

        [SugarColumn(Length = 500, IsNullable = false)]
        public string VALUE { get; set; }

        public DateTime CTIME { get; set; }
    }

    public class PT_TREE_DATA : PT_BASE
    {
        public int PARENTID { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string NAME { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string EXPLAIN { get; set; }

        [SugarColumn(Length = 500, IsNullable = false)]
        public string VALUE { get; set; }

        [SugarColumn(Length = 500, IsNullable = false)]
        public string PARENTS { get; set; }

        public int ORDERBY { get; set; }
    }

    [SqlSugar.SugarTable("PT_TREE_MENU")]
    public class PT_TREE_MENU : PT_TREE_DATA, I_TB_Base
    {
        [SugarColumn(Length = 50, IsNullable = false)]
        public string CLASS { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [SugarColumn(IsIgnore = true)]
        public string TABLE
        { get { return "PT_TREE_MENU"; } }

        [SugarColumn(IsIgnore = true)]
        public string PARENTNAME { get; set; }
    }

    [SqlSugar.SugarTable("PT_DB")]
    public class PT_DB : PT_BASE, I_TB_Base
    {
        [SugarColumn(Length = 50, IsNullable = false)]
        public string NAME { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string EXPLAIN { get; set; }

        [SugarColumn(Length = 500, IsNullable = false)]
        public string VALUE { get; set; }
    }

    #endregion 基础库表
}