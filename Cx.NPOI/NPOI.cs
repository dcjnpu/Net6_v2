using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Text;
using System.Web;

namespace Cx.NPOI
{
    public class ExcelHelper
    {
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export_Xls(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream Export_Xls(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            //IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            #region 右击文件 属性信息

            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息

                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }

            #endregion 右击文件 属性信息

            HSSFCellStyle? dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat? format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle!.DataFormat = format!.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()??"").Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            //取得列宽设计2
            //int[] arrColWidth = new int[dtSource.Columns.Count];
            //string cellstr = string.Empty; int intTemp = 0;
            //for (int j = 0; j < dtSource.Columns.Count; j++)
            //{
            //    cellstr = dtSource.Rows[0][j].ToString();
            //    if (cellstr.IndexOf("\n") > 0)
            //    {
            //        string[] cs = cellstr.Split('\n');
            //        cellstr = cs[0];
            //        for (int i = 1; i < cs.Length; i++) if (cellstr.Length < cs[i].Length) cellstr = cs[i];
            //    }
            //    intTemp = Encoding.GetEncoding(936).GetBytes(cellstr).Length;
            //    arrColWidth[j] = intTemp;
            //}

            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }
                    int i = 0;

                    #region 表头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(i) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        headerRow = null;
                        i++;
                    }

                    #endregion 表头及样式

                    #region 列头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(i) as HSSFRow;
                        headerRow.Height = 20 * 10 + 50;//单行
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                        headerRow = null;
                        i++;
                    }

                    #endregion 列头及样式

                    rowIndex = i;
                }

                #endregion 新建表，填充表头，填充列头，样式

                HSSFCellStyle bodystyle = workbook.CreateCellStyle() as HSSFCellStyle;
                //bodystyle.Alignment = HorizontalAlignment.CENTER;
                //bodystyle.WrapText = true;
                //bodystyle.VerticalAlignment = VerticalAlignment.CENTER;

                #region 填充内容

                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                    string drValue = row[column].ToString();

                    //if (drValue.IndexOf("#red#") > -1)
                    //{
                    //    //CellStyle style1 = hssfworkbook.CreateCellStyle();
                    //   bodystyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BLUE.index;
                    //  bodystyle.FillPattern = FillPatternType.BIG_SPOTS;
                    //    bodystyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.PINK.index;
                    //    //bodystyle.CreateRow(0).CreateCell(0).CellStyle = style1;
                    //}

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            newCell.CellStyle = bodystyle;
                            break;

                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示
                            break;

                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            newCell.CellStyle = bodystyle;
                            break;

                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            newCell.CellStyle = bodystyle;
                            break;

                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            newCell.CellStyle = bodystyle;
                            break;

                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            newCell.CellStyle = bodystyle;
                            break;

                        default:
                            newCell.SetCellValue("");
                            newCell.CellStyle = bodystyle;
                            break;
                    }
                }

                #endregion 填充内容

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                return ms;
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName)
        {
            //var curContext = Cx.Data.CxHttpContext.Current;
            var curContext = Cx.Data.CxHttpContextExtensions.Current;
            // 设置编码和附件格式
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.Headers.Add("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            //curContext.Response.BinaryWrite(Export_Xls(dtSource, strHeaderText).GetBuffer());
            //curContext.Response.End();
        }

        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (strFileName.Contains(".xlsx"))
                {
                    // 2007版本
                    hssfworkbook = new XSSFWorkbook(file);
                }
                else
                {
                    // 2003版本
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = getCellValue(row.GetCell(j));
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        public static string getCellValue(ICell cell)
        {
            // CellType cellType = cell.CellType;
            string cellValue = "";
            try
            {
                //if (cell.CellType == CellType.Formula)
                //{
                //    cell.SetCellType(CellType.Numeric);
                //    if (DateUtil.IsCellDateFormatted(cell))
                //    {
                //       cellValue = cell.DateCellValue;
                //    }
                //    else
                //    {
                //        cellValue = cell.StringCellValue;
                //    }
                //      return cellValue;
                //}

                if (cell.CellType == CellType.Formula) { cell.SetCellType(cell.CachedFormulaResultType); }

                if (cell.CellType == CellType.Numeric)
                {
                    if (DateUtil.IsCellDateFormatted(cell))
                        cellValue = cell.DateCellValue.ToString();
                    else cellValue = Convert.ToDecimal(cell.NumericCellValue).ToString();
                }
                else { cellValue = cell.ToString(); }
            }
            catch (Exception e) { cellValue = cell.ToString(); }

            return cellValue.Trim();
        }
    }
}