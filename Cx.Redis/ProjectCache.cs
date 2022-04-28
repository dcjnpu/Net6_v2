using CSRedis;
using Cx.Data;
using Cx.Sql;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections;
using System.Data;

namespace Cx.Redis
{
    internal static class ProjectCacheSetting
    {
        internal static IDictionary<string, T_Cache_Redis> cacheset = new Dictionary<string, T_Cache_Redis>();

        static ProjectCacheSetting()
        {
            Init();
        }
        internal static void Init()
        {
            string str = "select * from PT_CACHE";
            var conModel = DbConnection.Default;
            DataTable? dt = GetTable(conModel, str);// Cx.Sql.Ms.SqlExecute.GetTableStr("DATA_CONFIG", str, null);

            if (dt == null || dt.Rows.Count == 0) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(dr["CKey"].ToString())) continue;
                cacheset[dr["CKey"].ToString() ?? ""] = new T_Cache_Redis()
                {
                    CKey = dr["CKey"].ToString() ?? "",
                    Conn = dr["Conn"].ToString() ?? "",
                    CTitle = dr["CTitle"].ToString() ?? "",
                    TableName = dt.Columns.Contains("TableName") ? dr["TableName"].ToString() ?? "" : "",
                    JustSql = dr["JustSql"].ToString() ?? "",
                    DataType = dr["DataType"].ToInt32(),
                    ShowKey = dr["ShowKey"].ToString() ?? "",
                    ShowValue = dr["ShowValue"].ToString() ?? "",
                    RefreshSecond = dr["RefreshSecond"].ToInt32(),
                };
            }
        }

        internal static DataTable? GetTable(DbConnectionEntity conn, string str)
        {
            if (conn.dbType.ToLower()=="mysql") return MySqlExecute.GetTableStr(conn.Connstr, str, null);
            else return SqlExecute.GetTableStr(conn.Connstr, str, null);
        }
    }

    /// <summary>
    /// 项目配置缓存
    /// </summary>
    public class ProjectCache : IProjectCache
    {
        private CSRedisClient _cached;

        public CSRedisClient cached => _cached;




        /// <summary>
        /// DI注入用
        /// </summary>
        public ProjectCache(IConfiguration configuration)
        {
            string connectstring = configuration.GetSection("Cache:Redis").Value;
            _cached = new CSRedis.CSRedisClient(connectstring);
            IsConnect = _cached.Ping();
            // Init();
        }

        public ProjectCache()
        {
            //("119.45.174.xx:6379,password=123456,defaultDatabase=0");
            string connectstring = Cx.Data.ConfigExtensions.AppSettings.GetSection("Cache:Redis").Value;
            _cached = new CSRedis.CSRedisClient(connectstring);
            IsConnect = _cached.Ping();

            // Init();
        }





        /// <summary>
        /// 只刷新缓存配置
        /// </summary>
        public void ReLoad_Setting()
        {
            ProjectCacheSetting.Init();
        }

        /// <summary>
        /// 重新刷新系统缓存配置，并重置该配置的缓存
        /// </summary>
        /// <param name="ckey"></param>
        public void ReLoad_Setting(string ckey)
        {
            ProjectCacheSetting.Init();
            ReLoad_One(ckey);
        }

        /// <summary>
        /// 重置该配置的缓存
        /// </summary>
        /// <param name="ckey"></param>
        public void ReLoad_One(string ckey)
        {
            Reload_DataTable(ckey);
            Reload_HashTable(ckey);
        }

        public bool IsConnect { get; private set; }

        /// <summary>
        /// 默认的缓存
        /// </summary>
        public static ProjectCache Default;

        static ProjectCache()
        {
            Default = new ProjectCache();
        }

        private void Log(string msg)
        {
            Logger.Default.Process("redis", "ProjectCache", msg);
        }

        /// <summary>
        /// 获取缓存配置
        /// </summary>
        /// <param name="ckey">缓存主键</param>
        /// <returns></returns>
        public T_Cache_Redis? GetCacheSetting(string ckey)
        {
            //if (cacheset == null || cacheset.Keys.Count == 0) Init();
            if (ProjectCacheSetting.cacheset == null || ProjectCacheSetting.cacheset.Keys.Count == 0) return null;
            if (ProjectCacheSetting.cacheset.Keys.Contains(ckey)) return ProjectCacheSetting.cacheset[ckey];
            else return null;
        }

        //private string GetConnect(string conn)
        //{
        //    return "DbConnection:" + conn;
        //}

        #region datatable

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private DataTable? GetTable(string key)
        {
            if (!ProjectCacheSetting.cacheset.ContainsKey(key)) return null;
            T_Cache_Redis? model = GetCacheSetting(key);
            if (model == null) return null;
            DataTable? dt = ProjectCacheSetting.GetTable(DbConnection.Get(model.Conn)!, model.JustSql);//SqlExecute.GetTableStr(GetConnect(model.Conn), model.JustSql, null);
            if (dt == null || dt.Rows.Count == 0) return null;
            return dt;
        }

        public async Task<DataTable?> GetDataTable(string key)
        {
            if (!IsConnect) return GetTable(key);
            try
            {
                //2.设置一个test的key 值为 "我是redis测试字段的值",已经存值在内存数据库中了
                string t = await _cached.GetAsync<string>(CacheHelper.GetKey(key, E_CacheDataType.datatable));

                if (string.IsNullOrEmpty(t))
                {
                    DataTable? dt = GetTable(key);
                    _Reload_DataTable(key, dt);
                    return dt;
                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<DataTable>(t);
                    }
                    catch (Exception e)
                    {
                        Log("获取缓存数据[key:" + key + "]失败。" + e.Message);
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return GetTable(key);
            }
        }

        public void Reload_DataTable(string key)
        {
            //DataTable? t = GetTable(key);
            _Reload_DataTable(key, null);
        }

        private void _Reload_DataTable(string key, DataTable? t)
        {
            var model = GetCacheSetting(key);
            if (model == null) return;
            if (IsConnect) if (t==null)
                {
                    _cached.Del(CacheHelper.GetKey(key, E_CacheDataType.datatable));
                    //_cached.Set(CacheHelper.GetKey(key, E_CacheDataType.datatable), "", model.RefreshSecond);
                }
                else _cached.Set(CacheHelper.GetKey(key, E_CacheDataType.datatable), JsonConvert.SerializeObject(t), model.RefreshSecond);
        }

        #endregion datatable

        #region hashtable

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Hashtable? _GetHashTable(string key)
        {
            if (!ProjectCacheSetting.cacheset.ContainsKey(key)) return null;
            T_Cache_Redis? model = GetCacheSetting(key);
            if (model == null || string.IsNullOrEmpty(model.ShowKey) || string.IsNullOrEmpty(model.ShowValue)) return null;
            DataTable? dt = ProjectCacheSetting.GetTable(DbConnection.Get(model.Conn)!, model.JustSql);//SqlExecute.GetTableStr(GetConnect(model.Conn), model.JustSql, null);
            if (dt == null || dt.Rows.Count == 0) return null;
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[model.ShowKey] != null)
                    ht.Add(dr[model.ShowKey]?.ToString() ?? "", dr[model.ShowValue]);
            }
            return ht;
        }

        public async Task<Hashtable?> GetHashTable(string key)
        {
            if (!IsConnect) return _GetHashTable(key);
            try
            {
                //2.设置一个test的key 值为 "我是redis测试字段的值",已经存值在内存数据库中了
                Hashtable t = await _cached.GetAsync<Hashtable>(CacheHelper.GetKey(key, E_CacheDataType.hashtable));
                if (t == null)
                {
                    Hashtable? dt = _GetHashTable(key);
                    Reload_HashTable(key, dt);
                    return dt;
                }
                else
                {
                    return t;
                }
            }
            catch (Exception e)
            {
                return _GetHashTable(key);
            }
        }

        public void Reload_HashTable(string key)
        {
            //Hashtable? t = _GetHashTable(key);
            Reload_HashTable(key, null);
        }

        private void Reload_HashTable(string key, Hashtable? t)
        {
            var model = GetCacheSetting(key);
            if (model == null) return;
            if (IsConnect) { _cached.Set(CacheHelper.GetKey(key, E_CacheDataType.hashtable), t, model.RefreshSecond); }
        }

        #endregion hashtable
    }

    public static class ProjectCacheExsension
    {
        private static async Task<List<T>> GetFromBase<T>(IProjectCache projectCache, string key) where T : class, new()
        {
            var dt = await projectCache.GetDataTable(key);
            if (dt==null||dt.Rows.Count==0) return new List<T>();
            return (List<T>)Cx.Data.DataUtils.ConvertToList<T>(dt);
        }

        public static async Task<List<T>> GetList<T>(this IProjectCache projectCache, string key) where T : class, new()
        {
            try
            {
                if (!projectCache.IsConnect) throw new Exception("client未连接");
                //2.设置一个test的key 值为 "我是redis测试字段的值",已经存值在内存数据库中了
                var t = await projectCache.cached.GetAsync<List<T>>(CacheHelper.GetKey(key, E_CacheDataType.datatable));
                if (t==null) throw new Exception("缓存不存在");
                return t;
            }
            catch (Exception e)
            {
                return await GetFromBase<T>(projectCache, key);
            }
        }
    }

    /// <summary>
    /// 项目配置缓存
    /// </summary>
    public interface IProjectCache
    {
        CSRedisClient cached { get; }

        void ReLoad_Setting();

        /// <summary>
        /// 重新刷新系统缓存配置，并重置该配置的缓存
        /// </summary>
        /// <param name="ckey"></param>
        void ReLoad_Setting(string ckey);

        /// <summary>
        /// 重置该配置的缓存
        /// </summary>
        /// <param name="ckey"></param>
        void ReLoad_One(string ckey);

        public bool IsConnect { get; }

        /// <summary>
        /// 返回配置
        /// </summary>
        /// <param name="ckey"></param>
        /// <returns></returns>
        T_Cache_Redis? GetCacheSetting(string ckey);

        Task<DataTable?> GetDataTable(string key);

        void Reload_DataTable(string key);

        Task<Hashtable?> GetHashTable(string key);

        void Reload_HashTable(string key);
    }
}