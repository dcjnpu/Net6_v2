using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Cx.Data
{
    /// <summary>
    /// xml文件有要求：<head><item><key>value<key><item></head>
    /// </summary>
    public class XmlSettingReader
    {
        /// <summary>
        /// 读取xml配置文件，要求格式<head><item><key>value</key></item></head>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<List<TKeyValue>> Reader3(string path)
        {
            string purchaseOrderFilepath = FileHelper.GetServerPath(path);

            XElement purchaseOrder = XElement.Load(purchaseOrderFilepath);

            return await Task.Run(() =>
             {
                 var Value = new List<TKeyValue>();
                 var items = purchaseOrder.Elements();
                 foreach (var item in items)
                 {
                     var item_value = new Dictionary<string, string>();
                     var item_items = item.Elements();
                     foreach (var itema in item_items)
                     {
                         item_value.Add(itema.Name.ToString(), itema.Value);
                     }
                     TKeyValue model = new TKeyValue() { Key = item.Name.ToString(), Value = item_value };
                     Value.Add(model);
                 }
                 return Value;
             });
        }

        /// <summary>
        /// 读取xml配置文件，要求格式<head><key>value</key></head>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<Dictionary<string, string>> Reader2(string path)
        {
            string purchaseOrderFilepath = FileHelper.GetServerPath(path);

            XElement purchaseOrder = XElement.Load(purchaseOrderFilepath);

            return await Task.Run(() =>
            {
                var back = new Dictionary<string, string>();
                var items = purchaseOrder.Elements();
                foreach (var item in items)
                {
                    back.Add(item.Name.ToString(), item.Value);
                }
                return back;
            });
        }
    }


}
