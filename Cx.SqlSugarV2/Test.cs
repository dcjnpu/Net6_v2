using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.SqlSugarV2
{
    public class Test
    {
        public static void Test1()
        {
            using (var db = SugarHelper.Get("config"))
            {
                //db.GetModelAsync
            }
        }
    }
}
