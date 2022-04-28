using System.Linq.Expressions;
using Cx.Data;
using SqlSugar;

namespace CxSqlSugar
{
    public class BaseService : ISqlSugererver
    {
        /// <summary>
        /// 如果用的是DI，用CreateChild返回指定的BaseService
        /// </summary>
        //public BaseService(ILog)
        public BaseService()
        {
            //string dbtype = Cx.Data.ConfigExtensions.AppSettings["DbConnection:DbType"];
            //DbType _DbType = dbtype.ToLower() == "mysql" ? DbType.MySql : DbType.SqlServer;
            var config = DbConnection.Get("main")!;
            //if (config==null) return;
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = config.Connstr,
                DbType = config.EDbType,
                IsAutoCloseConnection = true
            });
        }

        public ISqlSugererver CreateChild(string dbkey)
        {
            return new BaseService(dbkey);
        }

        public BaseService(string dbkey)
        {
            var config = DbConnection.Get(dbkey)!;
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = config.Connstr,
                DbType = config.EDbType,
                IsAutoCloseConnection = true
            });
            //调式代码 用来打印SQL
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //string s = sql;
                //Console.WriteLine(sql + "\r\n" +
                //    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
            Db.Aop.OnError= (ex) =>
            {
                Logger.Default.Error("sqlsugar执行失败", ex);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <param name="dbtype"></param>
        /// <param name="flag">是否记录SQL</param>
        public BaseService(string connectionstring, DbType dbtype, bool flag)
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                //ConnectionString = ConfigExtensions.AppSettings["DbConnection:MySqlConnectionString"],
                ConnectionString = connectionstring,
                //DbType = DbType.MySql,
                DbType = dbtype,
                IsAutoCloseConnection = true
            });
            //调式代码 用来打印SQL
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (flag)
                {
                    string s = sql;
                    Console.WriteLine(sql + "\r\n" +
                        Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                }
            };
        }

        public void CreateTable(bool Backup = false, int StringDefaultLength = 50, params Type[] types)
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

        public SqlSugarClient Db { get; set; }

        #region 添加操作

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> AddAsync<T>(T parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                Db.Insertable<T>(parm);
                int dbres = Async ? await Db.Insertable<T>(parm).ExecuteReturnIdentityAsync() : Db.Insertable<T>(parm).ExecuteReturnIdentity();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> AddListAsync<T>(List<T> parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var dbres = Async ? await Db.Insertable<T>(parm).ExecuteCommandAsync() : Db.Insertable<T>(parm).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        #endregion 添加操作

        #region 查询操作

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> GetModelAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var data = Async ? await Db.Queryable<T>().Where(where).FirstAsync()
                : Db.Queryable<T>().Where(where).First();
            if (data==null) return new ApiResult<T> { success=true, statusCode= (int)ApiEnum.DataNull, data=new T() { } };
            else return new ApiResult<T> { success=true, statusCode= (int)ApiEnum.Status, data=data };
        }

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> GetModelAsync<T>(string parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<T>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var data = Async ? await Db.Queryable<T>().Where(parm).FirstAsync()
                    : Db.Queryable<T>().Where(parm).First();

                if (data==null) { res.statusCode= (int)ApiEnum.DataNull; data=new T() { }; }
                else { res.statusCode= (int)ApiEnum.Status; res.data=data; }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
		/// 获得列表——分页
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public async Task<ApiResult<Page<T>>> GetPagesAsync<T>(PageParm parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<Page<T>>();
            try
            {
                res.data = Async ? await Db.Queryable<T>()
                        .ToPageAsync(parm.page, parm.limit) : Db.Queryable<T>()
                        .ToPage(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderEnum">排序方式OrderByType</param>
        /// <returns></returns>
        public async Task<ApiResult<Page<T>>> GetPagesAsync<T>(PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<Page<T>>();
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                res.data = Async ? await query.ToPageAsync(parm.page, parm.limit) : query.ToPage(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
		/// 获得列表
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public async Task<ApiResult<List<T>>> GetListAsync<T>(Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<List<T>>();
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                res.data = Async ? await query.ToListAsync() : query.ToList();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<T>>> GetListAsync<T>(bool Async = true) where T : class, new()
        {
            var res = new ApiResult<List<T>>();
            try
            {
                res.data = Async ? await Db.Queryable<T>().ToListAsync() : Db.Queryable<T>().ToList();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        #endregion 查询操作

        #region 修改操作

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> UpdateAsync<T>(T parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var dbres = Async ? await Db.Updateable<T>(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() : Db.Updateable<T>(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> UpdateAsync<T>(List<T> parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var dbres = Async ? await Db.Updateable<T>(parm).ExecuteCommandAsync() : Db.Updateable<T>(parm).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 修改一条数据，可用作假删除,不需要修改的列请置为null
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> UpdateAsync<T>(Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var dbres = Async ? await Db.Updateable<T>().IgnoreColumns(ignoreAllNullColumns: true).SetColumns(columns).Where(where).ExecuteCommandAsync()
                    : Db.Updateable<T>().IgnoreColumns(ignoreAllNullColumns: true).SetColumns(columns).Where(where).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        //public async Task<ApiResult<string>> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
        //     bool Async = true) where T : class, new()
        //{
        //    var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
        //    try
        //    {
        //        var dbres = Async ? await Db.Updateable<T>().UpdateColumns(columns).ExecuteCommandAsync()
        //            : Db.Updateable<T>().UpdateColumns(columns).ExecuteCommand();
        //        res.data = dbres.ToString();
        //        res.statusCode = (int)ApiEnum.Status;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
        //        Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
        //    }
        //    return res;
        //}

        //public async Task<ApiResult<string>> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
        //   Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        //{
        //    var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
        //    try
        //    {
        //        var dbres = Async ? await Db.Updateable<T>().UpdateColumns(columns).Where(where).ExecuteCommandAsync()
        //            : Db.Updateable<T>().UpdateColumns(columns).Where(where).ExecuteCommand();
        //        res.data = dbres.ToString();
        //        res.statusCode = (int)ApiEnum.Status;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
        //        Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
        //    }
        //    return res;
        //}

        #endregion 修改操作

        #region 删除操作

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync<T>(string parm, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var list = DataUtils.StrToListString(parm);
                var dbres = Async ? await Db.Deleteable<T>().In(list.ToArray()).ExecuteCommandAsync() : Db.Deleteable<T>().In(list.ToArray()).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var dbres = Async ? await Db.Deleteable<T>().Where(where).ExecuteCommandAsync() : Db.Deleteable<T>().Where(where).ExecuteCommand();
                res.data = dbres.ToString();
                if (dbres>0) res.statusCode = (int)ApiEnum.Status;
                else res.statusCode = (int)ApiEnum.OprateFail;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        #endregion 删除操作

        #region 查询Count

        public async Task<ApiResult<ResultCount>> CountAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<ResultCount>() { statusCode = (int)ApiEnum.Error };
            try
            {
                res.data = new ResultCount
                {
                    Count = Async ? await Db.Queryable<T>().CountAsync(where) : Db.Queryable<T>().Count(where)
                };
                res.statusCode = (int)ApiEnum.Status;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        #endregion 查询Count

        #region 是否存在

        public async Task<ApiResult<ResultAny>> IsExistAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<ResultAny>() { statusCode = (int)ApiEnum.Error };
            try
            {
                res.data = new ResultAny
                {
                    Any = Async ? await Db.Queryable<T>().AnyAsync(where) : Db.Queryable<T>().Any(where)
                };
                res.statusCode = (int)ApiEnum.Status;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumDescription() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        #endregion 是否存在
    }
}