using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Linq.Expressions;
using Cx.Data;

namespace Cx.SqlSugarV2
{
    /// <summary>
    /// 新版配置，
    /// </summary>
    public static class SugarExtensions
    {
        #region ToPage
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrderBy"></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this ISugarQueryable<T> query,
            int pageIndex,
            int pageSize,
            bool isOrderBy = false)
        {
            RefAsync<int> totalItems = 0;
            var page = new Page<T>();
            page.Items = await query.ToPageListAsync(pageIndex, pageSize, totalItems);
            var totalPages = totalItems != 0 ? (totalItems % pageSize) == 0 ? (totalItems / pageSize) : (totalItems / pageSize) + 1 : 0;
            page.CurrentPage = pageIndex;
            page.ItemsPerPage = pageSize;
            page.TotalItems = totalItems;
            page.TotalPages = totalPages;
            return page;
        }


        public static async Task<Page<T>> ToPageAsync<T>(this ISugarQueryable<T> query,
           PageParm pageParm)
        {
            RefAsync<int> totalItems = 0;
            var page = new Page<T>();
            if (!string.IsNullOrEmpty(pageParm.field))
            {
                //var orderby = pageParm.order.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
                query.OrderBy(pageParm.field+" "+pageParm.order);
            }

            page.Items = await query.ToPageListAsync(pageParm.page, pageParm.limit, totalItems);
            var totalPages = totalItems != 0 ? (totalItems % pageParm.limit) == 0 ? (totalItems / pageParm.limit) : (totalItems / pageParm.limit) + 1 : 0;
            page.CurrentPage = pageParm.page;
            page.ItemsPerPage = pageParm.limit;
            page.TotalItems = totalItems;
            page.TotalPages = totalPages;
            return page;
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrderBy"></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this ISugarQueryable<T> query,
            int pageIndex,
            int pageSize,
            bool isOrderBy = false)
        {
            var page = new Page<T>();
            var totalItems = 0;
            page.Items = query.ToPageList(pageIndex, pageSize, ref totalItems);
            var totalPages = totalItems != 0 ? (totalItems % pageSize) == 0 ? (totalItems / pageSize) : (totalItems / pageSize) + 1 : 0;
            page.CurrentPage = pageIndex;
            page.ItemsPerPage = pageSize;
            page.TotalItems = totalItems;
            page.TotalPages = totalPages;
            return page;
        }

        #endregion

        #region 查询操作
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="Async"></param>
        /// <returns></returns>
        public static async Task<T?> GetModelAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            try
            {
                var data = Async ? await Db.Queryable<T>().Where(where).FirstAsync()
                : Db.Queryable<T>().Where(where).First();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public static async Task<T?> GetModelAsync<T>(this SqlSugarClient Db, string parm, bool Async = true) where T : class, new()
        {
            try
            {
                var data = Async ? await Db.Queryable<T>().Where(parm).FirstAsync()
                : Db.Queryable<T>().Where(parm).First();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 获得列表——分页
        /// </summary>
        /// <param name="parm">PageParm</param>
        /// <returns></returns>
        public static async Task<Page<T>> GetPagesAsync<T>(this SqlSugarClient Db, PageParm parm, bool Async = true) where T : class, new()
        {

            try
            {
                var data = Async ? await Db.Queryable<T>()
                        .ToPageAsync(parm.page, parm.limit) : Db.Queryable<T>()
                        .ToPage(parm.page, parm.limit);
                return data;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return new Page<T>() { CurrentPage = 1, ItemsPerPage = parm.limit, Items = new List<T>(), TotalItems = 0, TotalPages = 1 };
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderEnum">排序方式OrderByType</param>
        /// <returns></returns>
        public static async Task<Page<T>> GetPagesAsync<T>(this SqlSugarClient Db, PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new()
        {
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                return Async ? await query.ToPageAsync(parm.page, parm.limit) : query.ToPage(parm.page, parm.limit);
            }
            catch (Exception ex)
            {

                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return new Page<T>() { CurrentPage = 1, ItemsPerPage = parm.limit, Items = new List<T>(), TotalItems = 0, TotalPages = 1 };
        }

        /// <summary>
		/// 获得列表
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new()
        {
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                return Async ? await query.ToListAsync() : query.ToList();
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return new List<T>();
        }

        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(this SqlSugarClient Db, bool Async = true) where T : class, new()
        {
            try
            {
                return Async ? await Db.Queryable<T>().ToListAsync() : Db.Queryable<T>().ToList();
            }
            catch (Exception ex)
            {

                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return new List<T>();
        }

        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where, int Limit = 2000, bool Async = true) where T : class, new()
        {
            try
            {
                var query = Db.Queryable<T>().Where(where).Take(Limit);

                return Async ? await query.ToListAsync() : query.ToList();
            }
            catch (Exception ex)
            {

                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return new List<T>();
        }

        #endregion 查询操作

        #region 添加操作

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public static async Task<int> AddAsync<T>(this SqlSugarClient Db, T parm, bool Async = true) where T : class, new()
        {
            //var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                Db.Insertable<T>(parm);
                int dbres = Async ? await Db.Insertable<T>(parm).ExecuteReturnIdentityAsync() : Db.Insertable<T>(parm).ExecuteReturnIdentity();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public static async Task<int> AddListAsync<T>(this SqlSugarClient Db, List<T> parm, bool Async = true) where T : class, new()
        {
            try
            {
                var dbres = Async ? await Db.Insertable<T>(parm).ExecuteCommandAsync() : Db.Insertable<T>(parm).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return 0;
        }

        #endregion 添加操作

        #region 修改操作

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<T>(this SqlSugarClient Db, T parm, bool Async = true) where T : class, new()
        {
            try
            {
                var dbres = Async ? await Db.Updateable<T>(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() : Db.Updateable<T>(parm).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<T>(this SqlSugarClient Db, List<T> parm, bool Async = true) where T : class, new()
        {
            try
            {
                var dbres = Async ? await Db.Updateable<T>(parm).ExecuteCommandAsync() : Db.Updateable<T>(parm).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 修改一条数据，可用作假删除,不需要修改的列请置为null
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<T>(this SqlSugarClient Db, Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            try
            {
                //var dbres = Async ? await Db.Updateable<T>().IgnoreColumns(ignoreAllNullColumns: true).SetColumns(columns).Where(where).ExecuteCommandAsync()
                //    : Db.Updateable<T>().IgnoreColumns(ignoreAllNullColumns: true).SetColumns(columns).Where(where).ExecuteCommand();
                //return dbres;

                return Async ? await Db.Updateable<T>(columns).IgnoreColumns(ignoreAllNullColumns: true).Where(where).ExecuteCommandAsync()
                   : Db.Updateable<T>(columns).IgnoreColumns(ignoreAllNullColumns: true).Where(where).ExecuteCommand();
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        public static async Task<int> UpdateColumnsAsync<T>(this SqlSugarClient Db, Expression<Func<T, object>> columns, Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {

            try
            {
                var dbres = Async ? await Db.Updateable<T>().UpdateColumns(columns).Where(where).ExecuteCommandAsync()
                    : Db.Updateable<T>().UpdateColumns(columns).Where(where).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        public static async Task<int> UpdateColumnsAsync<T>(this SqlSugarClient Db, T t, Expression<Func<T, object>> columns,
             bool Async = true) where T : class, new()
        {

            try
            {
                var dbres = Async ? await Db.Updateable(t).UpdateColumns(columns).ExecuteCommandAsync()
                    : Db.Updateable<T>().UpdateColumns(columns).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        public static async Task<int> UpdateColumnsAsync<T>(this SqlSugarClient Db, Expression<Func<T, T>> columns,
           Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            try
            {
                var dbres = Async ? await Db.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommandAsync()
                    : Db.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        #endregion 修改操作

        #region 删除操作

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string  ,分隔的ID</param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this SqlSugarClient Db, string parm, bool Async = true) where T : class, new()
        {
            try
            {
                var list = DataUtils.StrToListString(parm);
                var dbres = Async ? await Db.Deleteable<T>().In(list.ToArray()).ExecuteCommandAsync() : Db.Deleteable<T>().In(list.ToArray()).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            try
            {
                var dbres = Async ? await Db.Deleteable<T>().Where(where).ExecuteCommandAsync() : Db.Deleteable<T>().Where(where).ExecuteCommand();
                return dbres;
            }
            catch (Exception ex)
            {

                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        #endregion 删除操作

        #region 查询Count

        public static async Task<int> CountAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {

            try
            {
                return Async ? await Db.Queryable<T>().CountAsync(where) : Db.Queryable<T>().Count(where);
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        #endregion 查询Count

        #region 是否存在

        public static async Task<int> IsExistAsync<T>(this SqlSugarClient Db, Expression<Func<T, bool>> where, bool Async = true) where T : class, new()
        {
            var res = new ApiResult<ResultAny>() { statusCode = (int)ApiEnum.Error };
            try
            {
                bool flag = Async ? await Db.Queryable<T>().AnyAsync(where) : Db.Queryable<T>().Any(where);
                return flag ? 1 : 0;
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return -1;
        }

        #endregion 是否存在

    }

}