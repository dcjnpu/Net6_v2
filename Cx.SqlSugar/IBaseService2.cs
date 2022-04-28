using SqlSugar;
using System.Linq.Expressions;

namespace CxSqlSugar
{
    public interface ISqlSugerServer
    {
        void CreateTable(bool Backup = false, int StringDefaultLength = 50, params Type[] types);

        ISqlSugerServer CreateChild(string dbkey, DbType dbtype);

        SqlSugarClient Db { get; set; }

        #region 添加操作

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<int> AddAsync<T>(T parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        Task<int> AddListAsync<T>(List<T> parm, bool Async = true) where T : class, new();


        #endregion 添加操作

        #region 查询操作

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<T?> GetModelAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();


        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        Task<T?> GetModelAsync<T>(string parm, bool Async = true) where T : class, new();


        /// <summary>
        /// 获得列表——分页
        /// </summary>
        /// <param name="parm">PageParm</param>
        /// <returns></returns>
        Task<Page<T>> GetPagesAsync<T>(PageParm parm, bool Async = true) where T : class, new();


        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderEnum">排序方式OrderByType</param>
        /// <returns></returns>
        Task<Page<T>> GetPagesAsync<T>(PageParm parm, Expression<Func<T, bool>> where,
           Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new();


        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="parm">PageParm</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where,
           Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true) where T : class, new();


        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>(bool Async = true) where T : class, new();


        #endregion 查询操作

        #region 修改操作

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T parm, bool Async = true) where T : class, new();

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(List<T> parm, bool Async = true) where T : class, new();


        /// <summary>
        /// 修改一条数据，可用作假删除,不需要修改的列请置为null
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(Expression<Func<T, T>> columns,
           Expression<Func<T, bool>> where, bool Async = true) where T : class, new();


        Task<int> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
            bool Async = true) where T : class, new();


        Task<int> UpdateColumnsAsync<T>(Expression<Func<T, object>> columns,
          Expression<Func<T, bool>> where, bool Async = true) where T : class, new();


        #endregion 修改操作

        #region 删除操作

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string  ,分隔的ID</param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(string parm, bool Async = true) where T : class, new();


        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();


        #endregion 删除操作

        #region 查询Count

        Task<int> CountAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();

        #endregion 查询Count

        #region 是否存在

        Task<int> IsExistAsync<T>(Expression<Func<T, bool>> where, bool Async = true) where T : class, new();


        #endregion 是否存在
    }
}