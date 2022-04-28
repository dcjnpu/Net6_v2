using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Linq.Expressions;
using Cx.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Cx.SqlSugarV2
{
    /// <summary>
    /// 新版配置，
    /// </summary>
    public static class CxSugarExtensions
    {
        /// <summary>
        /// Cx.Redis的DI
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddCxSugar(this IServiceCollection services)
        {
            return services.AddTransient<IPubSugar, PubSugar>();
        }

        public static SqlSugar.DbType EDbType(this DbConnectionEntity entity)
        {
            switch (entity.dbType.ToLower())
            {
                case "mysql": return SqlSugar.DbType.MySql;
                case "sqlserver": return SqlSugar.DbType.SqlServer;
                default: return SqlSugar.DbType.Custom;
            }
        }
    }

}