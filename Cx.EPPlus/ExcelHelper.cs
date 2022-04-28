using Cx.Data;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Data;
using System.Web;

namespace Cx.EPPlus
{
    /// <summary>
    /// EPPlus组件开发excel操作
    /// </summary>
    public class ExcelHelper
    {
        #region 测试方法

        /// <summary>
        /// 保存到本地方法 已测试，可以使用保存到本地后在下载的方式下载文件
        /// </summary>
        public static void TestUse()
        {
            DataTable dt = getData();
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\3.xlsx");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\" + Guid.NewGuid().ToString() + ".xlsx");
            FileInfo file = new FileInfo(path);
            if (file.Exists) file.Delete();
            ExcelHelper.Export(dt, (a =>
            {
                a.SaveAs(file);
            }), "tt");
        }

        /// <summary>
        /// 网页下载方法 待测试
        /// </summary>
        public static void TestUseWeb()
        {
            DataTable dt = getData();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\3.xlsx");
            FileInfo file = new FileInfo(path);
            if (file.Exists) file.Delete();
            ExcelHelper.Export(dt, (a =>
            {
                MemoryStream memoryStream = new MemoryStream();
                a.SaveAs(memoryStream);
                //转化为byte[]
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Seek(0, SeekOrigin.Begin);//没这句话就格式错
                //return bytes;
                //return memoryStream;
            }), "tt");
        }

        public static DataTable getData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("A1");
            dt.Columns.Add("B1");
            for (int i = 0; i < 3; i++)
            {
                DataRow DataRow = dt.NewRow();
                DataRow[0] = "a" + i; DataRow[1] = "b" + i; dt.Rows.Add(DataRow);
            }
            return dt;
        }

        #endregion 测试方法

        static ExcelHelper()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private static Type _type_datetime = typeof(DateTime);

        /// <summary>
        /// 只支持.xlsx的文件导入
        /// </summary>
        /// <param name="fullfilename"></param>
        /// <returns></returns>
        public static DataTable? Import(string fullfilename)
        {
            FileInfo file = new FileInfo(fullfilename);
            if (file == null || !file.Exists || file.Extension != ".xlsx") return null;

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //获取表格的列数和行数
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                DataTable dt = new DataTable();
                List<string> columstype = new List<string>();
                //设置列名
                for (int col = 1; col <= ColCount; col++)
                {
                    if (worksheet.Cells[1, col].Value == null) { ColCount = col - 1; break; }
                    dt.Columns.Add(worksheet.Cells[1, col].Value.ToString());
                    //columstype.Add((worksheet.Cells[2, col].Value ?? "").GetType().ToString());
                    columstype.Add(worksheet.Cells[2, col].Value?.GetType().ToString() ?? "null");
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    var dr = dt.NewRow();
                    object objectvalue;
                    for (int col = 1; col <= ColCount; col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        objectvalue = cell.Value;
                        //注意这里，是处理日期时间格式的关键代码
                        if (columstype[col - 1] == "null") { columstype[col - 1] = objectvalue?.GetType().ToString() ?? "null"; }

                        if (columstype[col - 1] == "System.Double")
                        {
                            if (cell.Style.Numberformat.Format.IndexOf("yy") > -1)
                                objectvalue = cell.GetValue<DateTime>();
                            //else objectvalue=cell.ToText();//toText()会导致数值不正确
                        }
                        else if (columstype[col - 1] == "System.DateTime") objectvalue = cell.GetValue<DateTime>();
                        dr[col - 1] = objectvalue == null ? "" : objectvalue.ToString();
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        /// <summary>
        /// 通用导出到excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="action"></param>
        /// <param name="strHeaderText"></param>
        public static void Export(DataTable dt, Action<ExcelPackage> action, string? strHeaderText)
        {
            int columscount = dt.Columns.Count;
            int rowcount = dt.Rows.Count;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                int currow = 1;
                if (!string.IsNullOrEmpty(strHeaderText))
                {
                    worksheet.Cells[1, 1, 1, columscount].Merge = true;

                    worksheet.Cells[1, 1].Value = strHeaderText;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;

                    currow++;
                }

                for (int i = 0; i < columscount; i++)
                {
                    var cell = worksheet.Cells[currow, i + 1];
                    cell.Value = dt.Columns[i].ColumnName;
                    cell.Style.Font.Bold = true;
                }
                currow++;

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columscount; i++)
                    {
                        var cell = worksheet.Cells[currow, i + 1];
                        cell.Value = dr[i];
                        if (dt.Columns[i].DataType == _type_datetime)
                        {
                            cell.Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                        }
                        else cell.Style.Numberformat.Format = "@";
                    }
                    currow++;
                }
                //package.SaveAs(file);
                action(package);
            }
        }

        public static void Export_FromIEnumerable<T>(IEnumerable<T> ts, Action<ExcelPackage> action, string strHeaderText)
        {
            var props = ShowNameAttrbute.GetColumns(typeof(T));
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                int currow = 1;
                if (!string.IsNullOrEmpty(strHeaderText))
                {
                    worksheet.Cells[1, 1, 1, props.Length].Merge = true;

                    worksheet.Cells[1, 1].Value = strHeaderText;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;

                    currow++;
                }

                for (int i = 0; i < props.Length; i++)
                {
                    var cell = worksheet.Cells[currow, i + 1];
                    cell.Value = ShowNameAttrbute.GetShowName(props[i]);
                    cell.Style.Font.Bold = true;
                    worksheet.Column(i + 1).AutoFit();
                }
                currow++;

                foreach (T t in ts)
                {
                    for (int i = 0; i<props.Length; i++)
                    {
                        object obj = props[i].GetValue(t, null)??"";
                        var cell = worksheet.Cells[currow, i + 1];
                        cell.Value = obj;
                        if (obj.GetType() == _type_datetime)
                        {
                            cell.Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                        }
                        else cell.Style.Numberformat.Format = "@";


                    }
                    currow++;
                }
                //package.SaveAs(file);
                action(package);
            }
        }


        /// <summary>
        /// 流导出
        /// </summary>
        /// <param name="package"></param>
        /// <param name="filename"></param>
        public static void HttpResponseFile(HttpContext httpContext, ExcelPackage package, string filename)
        {
            httpContext.Response.Headers.Append("Content-Disposition", "attachment;filename="
                + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8).ToString());
            httpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet; charset=UTF-8";
            package.SaveAs(httpContext.Response.Body);
        }

    }
}