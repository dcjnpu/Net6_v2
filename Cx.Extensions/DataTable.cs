using System.Data;
using System.Reflection;

namespace Cx.Extensions
{
    /// <summary>
    /// datatable 扩展操作
    /// </summary>
    public static class DataTableExtensions
    {
        #region DataTable=>List 方法2

        /// <summary>
        /// DataTable=>List 方法2 最好用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToList<T>(this DataTable dt) where T : new()
        {
            // 定义集合
            IList<T> ts = new List<T>();

            // 获得此模型的类型
            //Type type = typeof(T);
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();

            foreach (DataRow dr in dt.Rows)
            {
                t = new T();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            var pit = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                            var val = Convert.ChangeType(value, pit);
                            pi.SetValue(t, val, null);
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        /// <summary>
        /// DataTable=>List 方法2 最好用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T? ConvertToModel<T>(DataTable dt) where T : new()
        {
            // 定义集合
            var ts = ConvertToList<T>(dt);
            //if (ts == null || ts.Count() == 0) return default(T);
            return ts.FirstOrDefault<T>();
        }

        #endregion DataTable=>List 方法2
    }
}