using SqlSugar;
using System.Linq.Expressions;

namespace CxSqlSugar
{
    public interface ISqlSugererver
    {
        void CreateTable(bool Backup = false, int StringDefaultLength = 50, params Type[] types);

        ISqlSugererver CreateChild(string dbkey);

        SqlSugarClient Db { get; set; }

        #region 添加操作

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">cms_advlist</param>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync<T>(T parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        Task<ApiResult<string>> AddListAsync<T>(List<T> parm, bool Async = true) where T : class, new();

        #endregion 添加操作

        #region 查询操作

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <param name="order">Expression<Func<T, object>></param>
        /// <param name="orderEnum">DbOrderEnum</param>
        /// <returns></returns>
        Task<ApiResult<List<T>>> GetListAsync<T>(Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new();

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<List<T>>> GetListAsync<T>(bool Async = true) where T : class, new();

        /// <summary>
		/// 获得列表——分页
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
		Task<ApiResult<Page<T>>> GetPagesAsync<T>(PageParm parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderEnum">排序方式OrderByType</param>
        /// <returns></returns>
        Task<ApiResult<Page<T>>> GetPagesAsync<T>(PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new();

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        Task<ApiResult<T>> GetModelAsync<T>(string parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<ApiResult<T>> GetModelAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 查询操作

        #region 修改操作

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<ApiResult<string>> UpdateAsync<T>(T parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<ApiResult<string>> UpdateAsync<T>(List<T> parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 修改一条数据，可用作假删除
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        Task<ApiResult<string>> UpdateAsync<T>(Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        ///// <summary>
        ///// 修改一条数据，可用作假删除
        ///// </summary>
        ///// <param name="columns">修改的列=Expression<Func<T,T>></param>
        ///// <returns></returns>
        //Task<ApiResult<string>> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
        //        bool Async = true) where T : class, new();

        ///// <summary>
        ///// 修改一条数据，可用作假删除
        ///// </summary>
        ///// <param name="columns">修改的列=Expression<Func<T,T>></param>
        ///// <param name="where">Expression<Func<T,bool>></param>
        ///// <returns></returns>
        //Task<ApiResult<string>> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
        //    Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 修改操作

        #region 删除操作

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync<T>(string parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 删除操作

        #region 查询Count

        Task<ApiResult<ResultCount>> CountAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 查询Count

        #region 是否存在

        Task<ApiResult<ResultAny>> IsExistAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 是否存在
    }
}