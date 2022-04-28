using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// Excel导入导出操作
    /// </summary>
    public interface IExcelHelper
    {
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="fullfilename">文件路径全地址</param>
        /// <returns></returns>
        DataTable Import(string fullfilename);

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="strFileName"></param>
        void ExportByWeb(DataTable dtSource, string strHeaderText);
    }
}