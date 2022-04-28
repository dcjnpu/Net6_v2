using Cx.SqlSugarV2;
using System.Reflection;

namespace BaseBLL
{
    public class AdminStartup
    {


        /// <summary>
        /// 初始化所有基础表格
        /// </summary>
        public static void InitBaseTable(SqlSugar.SqlSugarClient Db)
        {
            try
            {
                //SqlDataBase._DB_Base = new BaseService2("DbConnection:MsSql", SqlSugar.DbType.MySql);
                Assembly assembly = Assembly.Load("BaseBLL");
                var ts = assembly.GetTypes().Where(u => u.IsClass && typeof(I_TB_Base).IsAssignableFrom(u)).ToArray();

                //DB_Base.CreateTable(false,50, typeof(PT_ADMIN));
                //初始化主表
                //foreach (var item in ts)
                //{
                SugarHelper.CreateTable(Db, false, 50, ts);
                // }

                //初始化日志表
                List<Type> ts2 = assembly.GetTypes().Where(u => u.IsClass && typeof(I_TB_Split_Base).IsAssignableFrom(u)).ToList();
                foreach (var item in ts2)
                {
                    Db!.CodeFirst.SplitTables().InitTables(item);
                }
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 以后再扩充
        /// </summary>
        public static void InitOtherTable(IPubSugar pubSugar)
        {
            try
            {
                //SqlDataBase._DB_Base = new BaseService2("DbConnection:MsSql", SqlSugar.DbType.MySql);
                //Assembly assembly = Assembly.Load("BaseBLL");
                //List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && typeof(I_TB_Area).IsAssignableFrom(u)).ToList();

                //var list = DbConnection.GetConnections().Where(h => h.Name.StartsWith("area_", StringComparison.OrdinalIgnoreCase)).ToList();
                ////DB_Base.CreateTable(false,50, typeof(PT_ADMIN));
                ////初始化主表
                //foreach (var item in ts)
                //{
                //    list.ForEach(h =>
                //    {
                //        var db = pubSugar.Get(h.Name);
                //        if (db != null)
                //            SugarHelper.CreateTable(db, false, 50, item);
                //    });
                //}
                ////初始化日志表
                //List<Type> ts2 = assembly.GetTypes().Where(u => u.IsClass && typeof(I_TB_Split_Base).IsAssignableFrom(u)).ToList();
                //foreach (var item in ts2)
                //{
                //    Db!.CodeFirst.SplitTables().InitTables(item);
                //}
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 初始化表数据
        /// </summary>
        public static async void InitTableData(SqlSugar.SqlSugarClient Db)
        {
            //菜单
            var cout1 = await Db.CountAsync<PT_TREE_MENU>(h => true);
            if (cout1 == 0)
            {
                List<PT_TREE_MENU> list = new List<PT_TREE_MENU>();
                list.Add(new PT_TREE_MENU { PARENTID = 0, NAME = "TOP_SYSTEM", EXPLAIN = "系统", VALUE = "1", PARENTS = "0,", ORDERBY = 1, CLASS = "layui-icon-util" });

                list.Add(new PT_TREE_MENU { PARENTID = 1, NAME = "LEFT_SYSSETTING", EXPLAIN = "配置", VALUE = "1", PARENTS = "0,1,", ORDERBY = 1, CLASS = "layui-icon-util" });

                list.Add(new PT_TREE_MENU { PARENTID = 2, NAME = "LIST_SYSCACHE", EXPLAIN = "系统缓存", VALUE = "~/admin/systemcache", PARENTS = "0,1,2,", ORDERBY = 1, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 2, NAME = "LIST_MENU", EXPLAIN = "后台菜单", VALUE = "~/admin/adminmenu", PARENTS = "0,1,2,", ORDERBY = 2, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 2, NAME = "LIST_SITEINFO", EXPLAIN = "站点信息", VALUE = "~/admin/siteinfo", PARENTS = "0,1,2,", ORDERBY = 3, CLASS = "" });

                //6
                list.Add(new PT_TREE_MENU { PARENTID = 1, NAME = "LEFT_ADMIN", EXPLAIN = "管理员", VALUE = "1", PARENTS = "0,1,", ORDERBY = 1, CLASS = "layui-icon-util" });
                list.Add(new PT_TREE_MENU { PARENTID = 6, NAME = "LIST_ADMINROLE", EXPLAIN = "角色", VALUE = "~/admin/roles", PARENTS = "0,1,6,", ORDERBY = 1, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 6, NAME = "LIST_ADMIN", EXPLAIN = "管理员列表", VALUE = "~/admin/admins", PARENTS = "0,1,6,", ORDERBY = 2, CLASS = "" });
                //9
                list.Add(new PT_TREE_MENU { PARENTID = 6, NAME = "POWER_ADMIN", EXPLAIN = "管理员权限", VALUE = "-1", PARENTS = "0,1,6,", ORDERBY = 3, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 9, NAME = "EDIT_ADMIN", EXPLAIN = "编辑权限", VALUE = "-1", PARENTS = "0,1,6,9,", ORDERBY = 1, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 9, NAME = "EDIT_ADMIN_PWD", EXPLAIN = "修改密码", VALUE = "-1", PARENTS = "0,1,6,9,", ORDERBY = 2, CLASS = "" });
                list.Add(new PT_TREE_MENU { PARENTID = 9, NAME = "DEIT_ADMIN_STATE", EXPLAIN = "帐号启停", VALUE = "-1", PARENTS = "0,1,6,9,", ORDERBY = 3, CLASS = "" });

                list.Add(new PT_TREE_MENU { PARENTID = 6, NAME = "LIST_ADMINLOG", EXPLAIN = "操作日志", VALUE = "~/admin/adminlog", PARENTS = "0,1,6,", ORDERBY = 4, CLASS = "" });

                list.Add(new PT_TREE_MENU { PARENTID = 0, NAME = "TOP_WEB", EXPLAIN = "系统配置", VALUE = "1", PARENTS = "0,", ORDERBY = 1, CLASS = "layui-icon-util" });

                await Db.AddListAsync<PT_TREE_MENU>(list);
            }

            //管理员帐号
            var cout2 = await Db.CountAsync<PT_ADMIN>(h => true);
            if (cout2 == 0)
            {
                PT_ADMIN a = new PT_ADMIN() { PWD = Cx.Data.DataUtils.MD5("123456"), REALNAME = "管理员", ROLEID = 1, UNAME = "admin", USTATE = 0, CTIME = DateTime.Now };
                await Db.AddAsync<PT_ADMIN>(a);
            }

            //角色
            var cout3 = await Db.CountAsync<PT_ROLE>(h => true);
            if (cout3 == 0)
            {
                PT_ROLE a = new PT_ROLE() { CTIME = DateTime.Now, EXPLAIN = "超级管理员", LEVEL = 100, NAME = "SuperAdmin", POWERS = "1111111111111111111111111111111111111111111111111111111111111111111111111" };
                await Db.AddAsync<PT_ROLE>(a);
            }

            //缓存
            var cout4 = await Db.CountAsync<PT_CACHE>(h => true);
            if (cout4 == 0)
            {
                List<PT_CACHE> list = new List<PT_CACHE>();
                list.Add(new PT_CACHE { CKey = "Http_AdminMenu", CTitle = "后台菜单", Conn = "config", TableName = "PT_TREE_MENU", JustSql = "SELECT * FROM PT_TREE_MENU ORDER BY PARENTID,ORDERBY", RefreshSecond = 3600, ShowKey = "NAME", ShowValue = "ID", DataType = 0, CTIME = DateTime.Now });
                list.Add(new PT_CACHE { CKey = "Http_Role", CTitle = "角色", Conn = "config", TableName = "PT_ROLE", JustSql = "SELECT * FROM PT_ROLE ORDER BY ID", RefreshSecond = 3600, ShowKey = "ID", ShowValue = "POWERS", DataType = 0, CTIME = DateTime.Now });
                list.Add(new PT_CACHE { CKey = "Http_SiteInfo", CTitle = "站点信息缓存", Conn = "config", TableName = "PT_SITEINFO", JustSql = "SELECT * FROM PT_SITEINFO ORDER BY ID", RefreshSecond = 3600, ShowKey = "NAME", ShowValue = "VALUE", DataType = 0, CTIME = DateTime.Now });
                await Db.AddListAsync<PT_CACHE>(list);
            }

            //站点信息
            var cout5 = await Db.CountAsync<PT_SITEINFO>(h => true);
            if (cout5 == 0)
            {
                List<PT_SITEINFO> list = new List<PT_SITEINFO>();
                list.Add(new PT_SITEINFO { NAME = "AdminUrl", EXPLAIN = "后台地址", VALUE = "http://ad.cx6.com/", CTIME = DateTime.Now });
                list.Add(new PT_SITEINFO { NAME = "SiteName", EXPLAIN = "网站名称", VALUE = "XX管理系统", CTIME = DateTime.Now });

                await Db.AddListAsync<PT_SITEINFO>(list);
            }
        }
    }
}