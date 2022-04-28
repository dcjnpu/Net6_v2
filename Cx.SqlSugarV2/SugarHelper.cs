using Cx.Data;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Cx.SqlSugarV2
{
    /// <summary>
    /// 新版配置，
    /// </summary>
    public class SugarHelper
    {
        /// <summary>
        /// 获取一个dbclient
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SqlSugarClient? Get(string key)
        {
            var connection = DbConnection.Get(key);
            if (connection==null) return null;
            var Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connection.Connstr,
                DbType =connection.EDbType(),
                IsAutoCloseConnection = true
            });
            //调式代码 用来打印SQL
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (connection.Needprintsql)
                {
                    string s = sql;
                    var str = sql + "\r\n" +
                        Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));

                    Console.WriteLine(str);
                    Logger.Default.Debug(str);
                }
            };

            return Db;
        }

        /// <summary>
        /// 建表
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="Backup"></param>
        /// <param name="StringDefaultLength"></param>
        /// <param name="types"></param>
        public static void CreateTable(SqlSugar.SqlSugarClient Db, bool Backup = false, int StringDefaultLength = 50, params Type[] types)
        {
            Db!.CodeFirst.SetStringDefaultLength(StringDefaultLength);
            Db.DbMaintenance.CreateDatabase();
            if (Backup)
            {
                Db.CodeFirst.BackupTable().InitTables(types);
            }
            else
            {
                Db.CodeFirst.InitTables(types);
            }
        }



    }




}