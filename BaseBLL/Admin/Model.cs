using Cx.Data;
using Cx.SqlSugarV2;

namespace BaseBLL
{
    public class SearchPagePam : PageParm
    {
        private int _roleid = 0;
        public int roleid { get => _roleid; set => _roleid = value; }
    }

    [SqlSugar.SugarTable("PT_ADMIN")]
    public class PT_ADMIN_PLUS : PT_ADMIN
    {
        public string ROLENAME { get; set; }

        public string USTATE_STR
        {
            get
            {
                return EnumDescriptionExtension.GetEnumDescription(typeof(E_USTATE), USTATE);
            }
        }
    }
}