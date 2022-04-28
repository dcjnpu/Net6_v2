using Microsoft.Data.SqlClient;
using System.Data;

namespace Cx.Sql
{
    interface ISqlExecute
    {

        #region 执行查询语句
        /// <summary>
        /// 执行查询语句，返回影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSql"></param>
        /// <param name="sps"></param>
        /// <returns></returns>
        Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps);

        Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps, int Arg);
        /// <summary>
        /// 执行查询语句，返回影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSql"></param>
        /// <param name="sps"></param>
        /// <returns></returns>
        Task<List<object>> ExecuteSql(string conn, string strSql, SqlParameter[] sps, List<int> Arg);


        #endregion

        #region 执行存储过程
        Task<int> Execute(string conn, string procedure, SqlParameter[] sps, int Arg);

        Task<List<object>> Execute(string conn, string procedure, SqlParameter[] sps, List<int> Arg);
        #endregion

        #region 查询表格

        DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps);

        DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result);

        DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps);
        DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result);
        #endregion

        #region 查询表格STRING

        DataTable? GetTableStr(string conn, string strSql, SqlParameter[]? sps);
        DataTable? GetTableStr(string conn, string strSql, SqlParameter[] sps, List<int> Arg, ref List<object> result);
        DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps);
        DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps, List<int>? Arg, ref List<object> result);
        #endregion

    }
}
