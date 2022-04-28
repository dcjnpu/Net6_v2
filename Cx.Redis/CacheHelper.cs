using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Redis
{
    public class CacheHelper
    {
        private static string top_key = Cx.Data.ConfigExtensions.AppSettings.GetSection("Cache:Redis_Top_Key").Value;
        public static string GetKey(string key, E_CacheDataType e)
        {
            if (e == E_CacheDataType.datatable) return top_key + "_dt_" + key;
            else if (e == E_CacheDataType.character) return top_key + "_st_" + key;
            return top_key + "_ht_" + key;
        }



    }

   
}
