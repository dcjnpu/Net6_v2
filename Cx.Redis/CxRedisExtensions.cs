using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Redis;
public static class CxRedisExtensions
{
    /// <summary>
    /// Cx.Redis的DI
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection AddCxRedis(this IServiceCollection services)
    {
        return services.AddSingleton<IProjectCache, ProjectCache>();
    }

    /// <summary>
    /// 从hashtable获取值
    /// </summary>
    /// <param name="ht"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object? GetFromHashtable(this Hashtable? ht, object key)
    {
        try
        {
            if (ht==null) return null;
            if (ht.ContainsKey(key)) return ht[key];
            return null;
        }
        catch
        {
            return null;
        }
    }


}

