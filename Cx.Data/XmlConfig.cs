using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Cx.Data
{
    public class XmlConfig
    {
        public async Task do1()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, @"config/setting.config");

            using (XmlReader reader = XmlReader.Create(purchaseOrderFilepath, settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine("Start Element {0}", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine("Text Node: {0}",
                                     await reader.GetValueAsync());
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine("End Element {0}", reader.Name);
                            break;
                        default:
                            Console.WriteLine("Other node {0} with value {1}",
                                            reader.NodeType, reader.Value);
                            break;
                    }
                }
            }
        }
    }
}