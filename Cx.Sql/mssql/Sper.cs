using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace Cx.Sql
{
    public class TSqler
    {
        public string Key = string.Empty;
        public SqlDbType DbType = SqlDbType.VarChar;
        public int Size = 0;
        public ParameterDirection Direction = ParameterDirection.Input;
    }

    /// <summary>
    /// 不再使用
    /// </summary>
    public class Sper
    {
        public static SqlParameter[] Get(List<TSqler> list, Hashtable ht, out int Arg)
        {
            Arg = -1;
            if (list == null || list.Count == 0 || ht == null || ht.Keys.Count == 0) return null;
            SqlParameter[] sps = new SqlParameter[list.Count];
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i] = new SqlParameter("@" + list[i].Key, list[i].DbType);
                if (list[i].Size > 0) sps[i].Size = list[i].Size;
                if (ht.ContainsKey(list[i].Key)) sps[i].Value = ht[list[i].Key];
                if (list[i].Direction != ParameterDirection.Input) sps[i].Direction = list[i].Direction;

                if ((list[i].Direction & ParameterDirection.Output) == ParameterDirection.Output) Arg = i;
            }
            return sps;
        }

        public static SqlParameter[] Get(List<TSqler> list, Hashtable ht, out List<int> Arg)
        {
            Arg = new List<int>();
            if (list == null || list.Count == 0 || ht == null || ht.Keys.Count == 0) return null;
            SqlParameter[] sps = new SqlParameter[list.Count];
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i] = new SqlParameter("@" + list[i].Key, list[i].DbType);
                if (list[i].Size > 0) sps[i].Size = list[i].Size;
                if (ht.ContainsKey(list[i].Key)) sps[i].Value = ht[list[i].Key];
                if (list[i].Direction != ParameterDirection.Input) sps[i].Direction = list[i].Direction;

                if ((list[i].Direction & ParameterDirection.Output) == ParameterDirection.Output) Arg.Add(i);
            }
            return sps;
        }
    }
}