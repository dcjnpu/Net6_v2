using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// 通用model key+value
    /// </summary>
    public class TKeyValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// 通用返回结构
    /// </summary>
    public class ApiBackModel
    {
        public int id { get; set; }
        public string msg { get; set; }
        public object? data { get; set; }

        public static ApiBackModel getRender(int _id, string _msg, object? _data = null)
        {
            return new ApiBackModel() { id = _id, msg = _msg, data = _data };
        }

        public static ApiBackModel getRender(long _id, string _msg, object? _data = null)
        {
            return new ApiBackModel() { id = _id.ToInt32(), msg = _msg, data = _data };
        }
    }

    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }

    #region 树结构

    /// <summary>
    /// 所有用树结构的表 统一接口格式
    /// </summary>
    public interface I_T_TREE_DATA
    {
        string TABLE { get; }

        int ID { get; set; }

        int PARENTID { get; set; }

        string PARENTNAME { get; set; }

        string NAME { get; set; }

        string EXPLAIN { get; set; }

        string VALUE { get; set; }

        string PARENTS { get; set; }

        int ORDERBY { get; set; }

        string UPSQL { get; }
    }

    /// <summary>
    /// 树结构-数据库基础结构
    /// </summary>
    public class T_TREE_DATA : I_T_TREE_DATA
    {
        private int _ID = 0;

        public int ID
        { get { return _ID; } set { _ID = value; } }

        private int _PARENTID = 0;

        public int PARENTID
        { get { return _PARENTID; } set { _PARENTID = value; } }

        private string _PARENTNAME = string.Empty;

        public string PARENTNAME
        { get { return _PARENTNAME; } set { _PARENTNAME = value; } }

        private string _NAME = string.Empty;

        public string NAME
        { get { return _NAME; } set { _NAME = value; } }

        private string _EXPLAIN = string.Empty;

        public string EXPLAIN
        { get { return _EXPLAIN; } set { _EXPLAIN = value; } }

        private string _VALUE = string.Empty;

        public string VALUE
        { get { return _VALUE; } set { _VALUE = value; } }

        private string _PARENTS = string.Empty;

        public string PARENTS
        { get { return _PARENTS; } set { _PARENTS = value; } }

        private int _ORDERBY = 0;

        public int ORDERBY
        { get { return _ORDERBY; } set { _ORDERBY = value; } }

        public virtual string UPSQL
        { get { return string.Empty; } }

        public virtual string TABLE
        { get { return string.Empty; } }
    }

    /// <summary>
    /// laytree数据结构,id 改为string型
    /// </summary>
    public class T_TREE
    {
        /// <summary>
        /// 节点唯一索引值，用于对指定节点进行各类操作
        /// </summary>
        public string? id { get; set; }

        /// <summary>
        /// 节点标题
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// 节点字段名
        /// </summary>
        public string? field { get; set; }

        public List<T_TREE>? children { get; set; }

        /// <summary>
        /// 点击节点弹出新窗口对应的 url。需开启 isJump 参数
        /// </summary>
        public string? href { get; set; }

        /// <summary>
        /// 节点是否初始展开，默认 false
        /// </summary>
        public bool spread { get; set; }

        /// <summary>
        /// 节点是否初始为选中状态（如果开启复选框的话），默认 false
        /// </summary>
        public bool @checked { get; set; }

        /// <summary>
        /// 节点是否为禁用状态。默认 false
        /// </summary>
        public bool disabled { get; set; }

        /// <summary>
        /// 外挂数据-所有信息
        /// </summary>
        public object? data { get; set; }
    }

    /// <summary>
    /// 菜单枚举
    /// </summary>
    public class T_TREE_DATA_MENU : T_TREE_DATA, I_T_TREE_DATA
    {
        public override string TABLE
        { get { return "PT_TREE_MENU"; } }

        //public override string UPSQL { get { return string.Empty; } }
    }

    #endregion 树结构
}