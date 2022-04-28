using Microsoft.Extensions.Configuration;

namespace Cx.Data
{
    /// <summary>
    /// 数据库连接字符串方法
    /// </summary>
    public class DbConnection
    {
        private static string _DbSettingPath = "appsettings.json";

        /// <summary>
        /// 数据库配置文件地址
        /// </summary>
        public static string DbSettingPath { get => _DbSettingPath; set => _DbSettingPath = value; }

        private static string _DefaulMainConnection = "config";

        /// <summary>
        /// 默认主链接库
        /// </summary>
        public static string DefaulMainConnection { get => _DefaulMainConnection; set => _DefaulMainConnection = value; }
        static DbConnection()
        {
            IConfiguration _configuration = new ConfigurationBuilder().AddJsonFile(DbSettingPath, true, true).Build();
            _configuration.GetSection("dbs").Bind(dbConnectionEntities2);

            DbConnectionEntity main = new();
            _configuration.GetSection("DbConnection").Bind(main);
            dbConnectionEntities2.Add(main);

            if (dbConnectionEntities2 != null && dbConnectionEntities2.Count > 0)
            {
                dbConnectionEntities2.ForEach(h =>
                {
                    dbConnectionEntities.Add(h.Name, h);
                });
            }

        }
        static List<DbConnectionEntity> dbConnectionEntities2 = new List<DbConnectionEntity>();
        static Dictionary<string, DbConnectionEntity> dbConnectionEntities = new Dictionary<string, DbConnectionEntity>();

        /// <summary>
        /// 根据name获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DbConnectionEntity? Get(string name)
        {
            dbConnectionEntities.TryGetValue(name, out DbConnectionEntity? rt);
            return rt;
        }

        /// <summary>
        /// 主配置库
        /// </summary>
        public static DbConnectionEntity Default
        {
            get
            {
                dbConnectionEntities.TryGetValue(DefaulMainConnection, out DbConnectionEntity? rt);
                return rt!;
            }
        }

        /// <summary>
        /// 获取所有链接配置
        /// </summary>
        /// <returns></returns>
        public static List<DbConnectionEntity> GetConnections()
        {
            return dbConnectionEntities2;
        }
    }

    /// <summary>
    /// appsetting.json的Dbs
    /// </summary>
    public class DbConnectionEntity
    {
        /// <summary>
        /// 数据库标识
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string Connstr { get; set; } = string.Empty;


        #region dbtype
        private string _dbType = "";
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string dbType { get => _dbType; set { _dbType = value; } }

        #endregion

        /// <summary>
        /// 设置是否打印sql语句
        /// </summary>
        public string Printsql { get => Needprintsql ? "true" : "false"; set { Needprintsql = value.ToLower().Equals("true"); } }
        /// <summary>
        /// 是否打印sql语句
        /// </summary>
        public bool Needprintsql { get; set; }
        /// <summary>
        /// 数据库说明
        /// </summary>
        public string Explain { get; set; } = string.Empty;
    }
}
