using Cx.Data;
using Cx.SqlSugarV2;

namespace BaseBLL
{
    public class CAdmin
    {
        public static void Log(string opid, string opname, E_AminLog e, string note, IPubSugar _sqlSugarHelper)
        {
            Log(new PT_LOG() { NOTE = note, OPID = opid, OPNAME = opname, OPTYPE = (int)e }, _sqlSugarHelper);
        }

        public static void Log(PT_LOG model, IPubSugar _sqlSugarHelper)
        {
            model.OPIP = CxHttpContextExtensions.IP();
            model.OPTIME = DateTime.Now;
            _sqlSugarHelper.Default.Insertable(model).SplitTable().ExecuteReturnSnowflakeIdAsync();
        }
    }
}