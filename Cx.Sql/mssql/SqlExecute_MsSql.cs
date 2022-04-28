using Microsoft.Data.SqlClient;
using System.Data;
using Cx.Data;

namespace Cx.Sql
{
    public class SqlExecute //: ISqlExecute
    {
        #region 自动计算返回值位置
        /// <summary>
        /// 获取返回值位置
        /// </summary>
        /// <param name="sps"></param>
        /// <returns></returns>
        public static int GetArg(SqlParameter[] sps)
        {
            List<int> rArgs = GetArgs(sps);
            if (rArgs == null || rArgs.Count == 0) return -1;
            return rArgs[0];
        }
        /// <summary>
        /// 获取返回值位置
        /// </summary>
        /// <param name="sps"></param>
        /// <returns></returns>
        public static List<int> GetArgs(SqlParameter[] sps)
        {
            if (sps == null || sps.Length == 0) return null;
            List<int> rArgs = new List<int>();
            for (int i = 0; i < sps.Length; i++)
            {
                if ((sps[i].Direction & ParameterDirection.Output) == ParameterDirection.Output)
                    rArgs.Add(i);
            }
            return rArgs;
        }
        #endregion


        const int Error_SqlExecute = -900001;
        private static void Log(string msg)
        {
            Logger.Default.Process("system", "SqlExecute.ExecuteSql", msg);
        }


        #region 执行查询语句
        /// <summary>
        /// 执行查询语句，返回影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSql"></param>
        /// <param name="sps"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps)
        {
            string connectionString = string.Empty;
            if (conn.IndexOf(';') <= 0) connectionString = ConfigExtensions.AppSettings.GetSection(conn).Value;
            else connectionString = conn;
            if (string.IsNullOrEmpty(connectionString)) { Log("获取数据库链接失败（" + conn + "）"); return 0; }
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                    PrepareCommand(cmd, conn2, null, CommandType.Text, strSql, sps);
                    int val = await cmd.ExecuteNonQueryAsync();
                    //清空SqlCommand中的参数列表
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    return val;
                }
                catch (Exception e) { Log("执行SQL查询失败。" + e.Message); return Error_SqlExecute; }
            }

        }
        public static async Task<int> ExecuteSql(string conn, string strSql, SqlParameter[] sps, int Arg)
        {
            List<object> result = await ExecuteSql(conn, strSql, sps, new List<int> { Arg });
            if (result == null || result.Count == 0) return 0;
            return result[0].ToInt32(0);
        }
        /// <summary>
        /// 执行查询语句，返回影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSql"></param>
        /// <param name="sps"></param>
        /// <returns></returns>
        public static async Task<List<object>> ExecuteSql(string conn, string strSql, SqlParameter[] sps, List<int> Arg)
        {
            string connectionString = string.Empty;
            if (conn.IndexOf(';') <= 0) connectionString = ConfigExtensions.AppSettings.GetSection(conn).Value;
            else connectionString = conn;
            if (string.IsNullOrEmpty(connectionString)) { Log("获取数据库链接失败（" + conn + "）"); return null; }
            List<object> result = new List<object>();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                    PrepareCommand(cmd, conn2, null, CommandType.Text, strSql, sps);
                    int val = await cmd.ExecuteNonQueryAsync();
                    //
                    if (Arg != null && Arg.Count > 0 && cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        foreach (int item in Arg)
                        {
                            if (cmd.Parameters.Count > item)
                                result.Add(cmd.Parameters[item].Value);
                        }
                    }
                    //清空SqlCommand中的参数列表
                    cmd.Parameters?.Clear();
                    cmd.Dispose();
                }
                catch (Exception e) { Log("执行SQL查询失败。" + e.Message); }
            }
            return result;
        }

        #endregion

        #region 执行存储过程
        public static async Task<int> Execute(string conn, string procedure, SqlParameter[] sps, int Arg)
        {
            List<object> result = await Execute(conn, procedure, sps, new List<int> { Arg });
            if (result == null || result.Count == 0) return 0;
            return SafeConvertExtensions.ToInt32(result[0]);
        }
        public static async Task<List<object>> Execute(string conn, string procedure, SqlParameter[] sps, List<int> Arg)
        {
            string connectionString = string.Empty;
            if (conn.IndexOf(';') <= 0) connectionString = ConfigExtensions.AppSettings.GetSection(conn).Value;
            else connectionString = conn;
            if (string.IsNullOrEmpty(connectionString)) { Log("获取数据库链接失败（" + conn + "）"); return null; }
            List<object> result = new List<object>();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                    PrepareCommand(cmd, conn2, null, CommandType.StoredProcedure, procedure, sps);
                    //int val = cmd.ExecuteNonQuery();
                    int val = await cmd.ExecuteNonQueryAsync();
                    if (Arg != null && Arg.Count > 0 && cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        foreach (int item in Arg)
                        {
                            if (cmd.Parameters.Count > item)
                                result.Add(cmd.Parameters[item].Value);
                        }
                    }
                }
                catch (Exception e) { Log("执行SQL查询失败。" + e.Message); }
                //清空SqlCommand中的参数列表
                cmd.Parameters?.Clear();
            }
            cmd.Dispose();
            return result;
        }
        #endregion

        #region 查询表格

        public static DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps)
        {
            List<object> result = new List<object>();
            DataTableCollection? dts = GetTables(conn, procedure, sps, null, ref result);
            if (dts == null || dts.Count == 0) return null;
            return dts[0];
        }
        public static DataTable? GetTable(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
        {
            DataTableCollection? dts = GetTables(conn, procedure, sps, Arg, ref result);
            if (dts == null || dts.Count == 0) return null;
            return dts[0];
        }
        public static DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps)
        {
            List<object> result = new List<object>();
            return GetTables(conn, procedure, sps, null, ref result);
        }
        public static DataTableCollection? GetTables(string conn, string procedure, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
        {
            string connectionString = string.Empty;
            if (conn.IndexOf(';') <= 0) connectionString = ConfigExtensions.AppSettings.GetSection(conn).Value;
            else connectionString = conn;

            if (string.IsNullOrEmpty(connectionString)) { Log("获取数据库链接失败（" + conn + "）"); return null; }
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    PrepareCommand(cmd, conn2, null, CommandType.StoredProcedure, procedure, sps);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;

                    adapter.Fill(ds);
                }
                catch (Exception e) { Log("执行SQL查询失败。" + e.Message); }
            }
            DataTableCollection table = ds.Tables;

            if (Arg != null && Arg.Count > 0 && cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                if (result == null) result = new List<object>();
                foreach (int item in Arg)
                {
                    if (cmd.Parameters.Count > item)
                        result.Add(cmd.Parameters[item].Value);
                }
            }
            return table;
        }
        #endregion

        #region 查询表格STRING

        public static DataTable? GetTableStr(string conn, string strSql, SqlParameter[]? sps)
        {
            List<object> result = new List<object>();
            DataTableCollection? dts = GetTablesStr(conn, strSql, sps, null, ref result);
            if (dts == null || dts.Count == 0) return null;
            return dts[0];
        }
        public static DataTable? GetTableStr(string conn, string strSql, SqlParameter[] sps, List<int> Arg, ref List<object> result)
        {
            DataTableCollection? dts = GetTablesStr(conn, strSql, sps, Arg, ref result);
            if (dts == null || dts.Count == 0) return null;
            return dts[0];
        }
        public static DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps)
        {
            List<object> result = new List<object>();
            return GetTablesStr(conn, strSql, sps, null, ref result);
        }
        public static DataTableCollection? GetTablesStr(string conn, string strSql, SqlParameter[]? sps, List<int>? Arg, ref List<object> result)
        {
            string connectionString = string.Empty;
            if (conn.IndexOf(';') <= 0) connectionString = ConfigExtensions.AppSettings.GetSection(conn).Value;
            else connectionString = conn;
            if (string.IsNullOrEmpty(connectionString)) { Log("获取数据库链接失败（" + conn + "）"); return null; }
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    PrepareCommand(cmd, conn2, null, CommandType.Text, strSql, sps);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                catch (Exception e) { Log("执行SQL查询失败。" + e.Message); }
            }
            DataTableCollection table = ds.Tables;
            if (Arg != null && Arg.Count > 0 && cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                if (result == null) result = new List<object>();
                foreach (int item in Arg)
                {
                    if (cmd.Parameters.Count > item)
                        result.Add(cmd.Parameters[item].Value);
                }
            }
            return table;
        }
        #endregion

        #region private
        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction? trans, CommandType cmdType, string cmdText, SqlParameter[]? cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion
    }
}