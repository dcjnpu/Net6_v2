using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.SqlSugarV2
{
    /// <summary>
    /// 用于IOC
    /// </summary>
    public class PubSugar : IPubSugar
    {
        public const string key_default = "config";

        public PubSugar() { }

        public SqlSugarClient? Get(string key)
        {
            return SugarHelper.Get(key);
        }

        

        public virtual SqlSugarClient GetBySplit(string splitkey)
        {
            throw new NotImplementedException("请重写该方法");
        }
        public virtual SqlSugarClient GetBySplit(int splitkey)
        {
            throw new NotImplementedException("请重写该方法");
        }

        public SqlSugarClient Default => SugarHelper.Get(key_default)!;
    }
    /// <summary>
    /// 用于IOC
    /// </summary>
    public interface IPubSugar
    {
        /// <summary>
        /// 获取指定库
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        SqlSugarClient? Get(string key);
        /// <summary>
        /// 配置库
        /// </summary>
        SqlSugarClient Default { get; }

        /// <summary>
        /// 获取分库
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        SqlSugarClient GetBySplit(string key);

        SqlSugarClient GetBySplit(int key);
    }
}
