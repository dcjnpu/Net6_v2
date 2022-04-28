using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using SqlSugar;

namespace CxSqlSugar
{
    /// <summary>
    /// 数据库连接字符串方法
    /// </summary>
    public class DbConnection
    {
        //private static readonly IConfiguration _configuration;
        static DbConnection()
        {
            IConfiguration _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            _configuration.GetSection("dbs").Bind(dbConnectionEntities2);

            DbConnectionEntity main = new DbConnectionEntity();
            _configuration.GetSection("DbConnection").Bind(main);
            dbConnectionEntities2.Add(main);

            if (dbConnectionEntities2!=null&&dbConnectionEntities2.Count>0)
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
        public string Name { get; set; } = string.Empty;
        public string Connstr { get; set; } = string.Empty;


        #region dbtype
        private string _dbType = "";
        public string dbType { get => _dbType; set { _dbType=value; SetEDbType(value); } }

        private DbType _eDbType = SqlSugar.DbType.Custom;

        private void SetEDbType(string value)
        {
            switch (value.ToLower())
            {
                case "mysql": EDbType=DbType.MySql; break;
                case "sqlserver": EDbType=DbType.SqlServer; break;
            }
        }

        public DbType EDbType
        {
            get => _eDbType; set => _eDbType=value;
        }
        #endregion

        public string Printsql { set { Needprintsql=value.ToLower().Equals("true"); } } 

        public bool Needprintsql { get; set; }
    }
}