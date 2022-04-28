using Microsoft.Data.SqlClient;
using System.Data;

namespace Cx.Sql
{
    //public partial class SqlExecute
    //{
    //    #region 自动计算返回值位置
    //    /// <summary>
    //    /// 获取返回值位置
    //    /// </summary>
    //    /// <param name="sps"></param>
    //    /// <returns></returns>
    //    public static int GetArg(SqlParameter[] sps)
    //    {
    //        List<int> rArgs = GetArgs(sps);
    //        if (rArgs == null || rArgs.Count == 0) return -1;
    //        return rArgs[0];
    //    }
    //    /// <summary>
    //    /// 获取返回值位置
    //    /// </summary>
    //    /// <param name="sps"></param>
    //    /// <returns></returns>
    //    public static List<int> GetArgs(SqlParameter[] sps)
    //    {
    //        if (sps == null || sps.Length == 0) return null;
    //        List<int> rArgs = new List<int>();
    //        for (int i = 0; i < sps.Length; i++)
    //        {
    //            if ((sps[i].Direction & ParameterDirection.Output) == ParameterDirection.Output)
    //                rArgs.Add(i);
    //        }
    //        return rArgs;
    //    }
    //    #endregion


    //    private static ISqlExecute ISql;

    //    static SqlExecute()
    //    {
    //        string dbtype = Cx.Data.ConfigExtensions.AppSettings["DbConnection:DbType"];
    //        if (dbtype.ToLower() == "mysql") ISql = new SqlExecute_MsSql();
    //        else ISql = new SqlExecute_MsSql();
    //    }

    //    #region 执行查询语句
    //    /// <summary>
    //    /// 执行查询语句，返回影响的行数
    //    /// </summary>
    //    /// <param name="conn"></param>
    //    /// <param name="strSql"></param>
    //    /// <param name="sps"></param>
    //    /// <returns></returns>
    //    public static Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps)
    //    {
    //        return ISql.ExecuteSql(conn, strSql, sps);
    //    }

    //    public static Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps, int Arg)
    //    {
    //        return ISql.ExecuteSql(conn, strSql, sps, Arg);
    //    }
    //    /// <summary>
    //    /// 执行查询语句，返回影响的行数
    //    /// </summary>
    //    /// <param name="conn"></param>
    //    /// <param name="strSql"></param>
    //    /// <param name="sps"></param>
    //    /// <returns></returns>
    //    public static Task<List<object>> ExecuteSql(string conn, string strSql, SqlParameter[] sps, List<int> Arg)
    //    {
    //        return ISql.ExecuteSql(conn, strSql, sps, Arg);
    //    }


    //    #endregion

    //    #region 执行存储过程
    //    public static Task<int> Execute(string conn, string procedure, SqlParameter[] sps, int Arg)
    //    {
    //        return ISql.Execute(conn, procedure, sps, Arg);
    //    }
    //    public static Task<List<object>> Execute(string conn, string procedure, SqlParameter[] sps, List<int> Arg)
    //    {
    //        return ISql.Execute(conn, procedure, sps, Arg);
    //    }
    //    #endregion

    //    #region 查询表格

    //    public static DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps)
    //    {
    //        return ISql.GetTable(conn, procedure, sps);
    //    }

    //    public static DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
    //    {
    //        return ISql.GetTable(conn, procedure, sps, Arg, ref result);
    //    }

    //    public static DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps)
    //    {
    //        return ISql.GetTables(conn, procedure, sps);
    //    }
    //    public static DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
    //    {
    //        return ISql.GetTables(conn, procedure, sps, Arg, ref result);
    //    }
    //    #endregion

    //    #region 查询表格STRING

    //    public static DataTable? GetTableStr(string conn, string strSql, SqlParameter[]? sps)
    //    {
    //        return ISql.GetTableStr(conn, strSql, sps);
    //    }
    //    public static DataTable? GetTableStr(string conn, string strSql, SqlParameter[] sps, List<int> Arg, ref List<object> result)
    //    {
    //        return ISql.GetTableStr(conn, strSql, sps, Arg, ref result);
    //    }
    //    public static DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps)
    //    {
    //        return ISql.GetTablesStr(conn, strSql, sps);
    //    }
    //    public static DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
    //    {
    //        return ISql.GetTablesStr(conn, strSql, sps, Arg, ref result);
    //    }
    //    #endregion

    //}
}