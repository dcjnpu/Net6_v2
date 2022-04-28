using CSRedis;
namespace Cx.Redis
{
    /// <summary>
    /// 一般缓存
    /// </summary>
    public class Caching
    {
        public static Caching Default;
        static Caching()
        {
            Default = new Caching();
        }

        public CSRedisClient Redis;
        public Caching()
        {
            //("119.45.174.xx:6379,password=123456,defaultDatabase=0");
            string connectstring = Cx.Data.ConfigExtensions.AppSettings.GetSection("Cache:Redis").Value;
            Redis = new CSRedis.CSRedisClient(connectstring);

        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnect { get => Redis.Ping(); }

    }
}