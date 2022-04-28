using BaseBLL;
using Cx.Data;
using Cx.Redis;
using Cx.SqlSugarV2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Webpage.Models;

namespace Webpage.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IPubSugar _sqlSugarHelper;
        private readonly IProjectCache _projectCache;
        private readonly Admin_MyJwt _admin;

        public AdminController(Admin_MyJwt admin, IPubSugar sqlSugarHelper, IProjectCache projectCache)
        {
            _sqlSugarHelper = sqlSugarHelper;
            _projectCache = projectCache;
            _admin = admin;
        }

        #region 菜单配置

        public IActionResult AdminMenu()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMenu(int id, string ckey)
        {
            if (string.IsNullOrEmpty(ckey)) return Json(ApiBackModel.getRender(-1, "ckey不能为空"));
            DataTable? dt = await _projectCache.GetDataTable(ckey);

            if (dt == null || dt.Rows.Count == 0)
            {
                T_TREE tree1 = new T_TREE() { title = "根节点", id = "0", children = null, spread = true, href = "http://www.baidu.com", data = new T_TREE_DATA() { EXPLAIN = "根节点", ID = 0 } };
                List<T_TREE> back1 = new List<T_TREE>() { tree1 };

                return Json(ApiBackModel.getRender(1, "", back1));
            }
            int cuID = id;
            dt.Columns.Add("PARENTNAME");
            foreach (DataRow dr in dt.Rows)
            {
                dr["PARENTNAME"] = GetParentName((dr["PARENTID"].ToInt32()), dt);
            }

            IList<PT_TREE_MENU> list = null;
            try
            {
                list = Cx.Data.DataUtils.ConvertToList<PT_TREE_MENU>(dt);
            }
            catch (Exception e)
            {
                list = null;
            }
            if (list == null || list.Count == 0) { return Json(ApiBackModel.getRender(-11, "没有节点")); }
            bool spread = false;
            T_TREE tree = new T_TREE() { title = "根节点", id = "0", children = getChilds(0, cuID, list, ref spread), spread = true, href = "http://www.baidu.com", data = new T_TREE_DATA() { EXPLAIN = "根节点", ID = 0 } };
            List<T_TREE> back = new List<T_TREE>() { tree };
            var backm = ApiBackModel.getRender(1, "", back);
            return Json(JsonConvert.SerializeObject(backm));
        }

        private string GetParentName(int id, DataTable dt)
        {
            if (id == 0) return "根节点";
            DataRow[] drs = dt.Select("ID=" + id);
            if (drs == null || drs.Count() == 0) return string.Empty;
            return drs[0]["EXPLAIN"]?.ToString() ?? "";
        }

        private List<T_TREE> getChilds(int parentid, int cuid, IList<PT_TREE_MENU> list, ref bool spread)
        {
            var a = list.Where(h => h.PARENTID == parentid).OrderBy(h => h.ORDERBY);
            if (a == null) return null;
            List<PT_TREE_MENU> list2 = a.ToList();

            if (list2 != null && list2.Count > 0)
            {
                List<T_TREE> back = new List<T_TREE>();

                foreach (PT_TREE_DATA m in list2)
                {
                    if (parentid == 0) spread = false;
                    back.Add(new T_TREE()
                    {
                        id = m.ID.ToString(),
                        title = m.EXPLAIN + "," + m.NAME,
                        children = getChilds(m.ID.ToInt32(), cuid, list, ref spread),
                        href = "http://www.baidu.com",
                        spread = spread,
                        data = m
                    });
                    if (!spread && m.ID == cuid) spread = true;
                }
                return back;
            }
            else return null;
        }

        [HttpPost]
        public async Task<IActionResult> DelMenu(int id)
        {
            //if (!HasPower("LIST_MENU")) { Render(-11, "权限不足"); return; }
            //int ID = CxConvert.ToInt(CxContext.Query("id"));
            var tmenu = await _sqlSugarHelper.Default.GetModelAsync<PT_TREE_MENU>(h => h.ID == id);

            var childs = await _sqlSugarHelper.Default.GetListAsync<PT_TREE_MENU>((h => h.PARENTID == id), (h => h.ID), DbOrderEnum.Asc);
            if (childs != null && childs.Count > 0) { return Json(ApiBackModel.getRender(-11, "需要先删除子节点")); }

            var rInt = await _sqlSugarHelper.Default.DeleteAsync<PT_TREE_MENU>(h => h.ID == id);
            var tUser = _admin.Get();
            CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.adminmenu, "删除菜单信息成功(" + JsonConvert.SerializeObject(tmenu) + ")", _sqlSugarHelper);

            return Json(ApiBackModel.getRender(1, "删除成功"));
        }

        [HttpPost]
        public async Task<IActionResult> EditMenu(PT_TREE_MENU model)
        {
            if (model == null) { return Json(ApiBackModel.getRender(-11, "数据错误")); }

            if (model.ID == 0)
            {
                if (model.PARENTID == 0)
                {
                    model.PARENTS = "0,";
                }
                else
                {
                    var model1 = await _sqlSugarHelper.Default.GetModelAsync<PT_TREE_MENU>(h => h.ID == model.PARENTID);
                    model.PARENTS = model1?.PARENTS ?? "0," + model.PARENTID + ",";
                }
                var back = await _sqlSugarHelper.Default.AddAsync<PT_TREE_MENU>(model);
                model.ID = back;
            }
            else
            {
                //var api= await _sqlSugarHelper.Default.GetModelAsync<PT_TREE_MENU>(h=>h.ID==model.ID);
                //var vmodel = api.data; vmodel.
                await _sqlSugarHelper.Default.UpdateAsync<PT_TREE_MENU>(model);
            }
            _projectCache.ReLoad_One(AdminConst.Http_AdminMenu);
            var tUser = _admin.Get();
            CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.adminmenu, "保存菜单信息成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);

            return Json(ApiBackModel.getRender(model.ID.ToInt32(), "操作成功"));
        }

        #endregion 菜单配置

        #region 树结构操作

        [HttpPost]
        public async Task<IActionResult> GetTree(int id, string ckey)
        {
            if (string.IsNullOrEmpty(ckey)) return Json(ApiBackModel.getRender(-1, "ckey不能为空"));
            DataTable? dt = await ProjectCache.Default.GetDataTable(ckey);

            if (dt == null || dt.Rows.Count == 0)
            {
                T_TREE tree1 = new T_TREE() { title = "根节点", id = "0", children = null, spread = true, href = "http://www.baidu.com", data = new T_TREE_DATA() { EXPLAIN = "根节点", ID = 0 } };
                List<T_TREE> back1 = new List<T_TREE>() { tree1 };

                return Json(ApiBackModel.getRender(1, "", back1));
            }
            int cuID = id;

            IList<PT_TREE_DATA> list = null;
            try
            {
                list = Cx.Data.DataUtils.ConvertToList<PT_TREE_DATA>(dt);
            }
            catch (Exception e)
            {
                list = null;
            }
            if (list == null || list.Count == 0) { return Json(ApiBackModel.getRender(-11, "没有节点")); }
            bool spread = false;
            T_TREE tree = new T_TREE() { title = "根节点", id = "0", children = getChilds(0, cuID, list, ref spread), spread = true, href = "http://www.baidu.com", data = new T_TREE_DATA() { EXPLAIN = "根节点", ID = 0 } };
            List<T_TREE> back = new List<T_TREE>() { tree };
            var backm = ApiBackModel.getRender(1, "", back);
            return Json(JsonConvert.SerializeObject(backm));
        }

        private List<T_TREE> getChilds(int parentid, int cuid, IList<PT_TREE_DATA> list, ref bool spread)
        {
            var a = list.Where(h => h.PARENTID == parentid).OrderBy(h => h.ORDERBY);
            if (a == null) return null;
            List<PT_TREE_DATA> list2 = a.ToList();

            if (list2 != null && list2.Count > 0)
            {
                List<T_TREE> back = new List<T_TREE>();

                foreach (PT_TREE_DATA m in list2)
                {
                    if (parentid == 0) spread = false;
                    back.Add(new T_TREE()
                    {
                        id = m.ID.ToString(),
                        title = m.EXPLAIN + "," + m.NAME,
                        children = getChilds(m.ID.ToInt32(), cuid, list, ref spread),
                        href = "http://www.baidu.com",
                        spread = spread,
                        data = m
                    });
                    if (!spread && m.ID == cuid) spread = true;
                }
                return back;
            }
            else return null;
        }

        public async Task<IActionResult> Tree(string ckey)
        {
            ViewBag.Ckey = ckey;
            return View();
        }

        #endregion 树结构操作

        #region siteinfo

        public IActionResult SiteInfo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSiteInfo(PageParm model)
        {
            //model.page = page;
            //model.limit = limit;
            var where = ExpressionFunc.GetTrue<PT_SITEINFO>();
            if (!string.IsNullOrEmpty(model.key))
            {
                where = where.And(h => h.NAME.Contains(model.key) || h.EXPLAIN.Contains(model.key));
            }
            var apiResult = await _sqlSugarHelper.Default.GetPagesAsync<PT_SITEINFO>(model, where, h => h.ID, DbOrderEnum.Asc);
            return Json(PageHelper.GetPage(apiResult));
        }

        public async Task<IActionResult> SiteInfo_Add(int id)
        {
            if (id <= 0)
            {
                return View(new PT_SITEINFO());
            }
            else
            {
                var apiResult = await _sqlSugarHelper.Default.GetModelAsync<PT_SITEINFO>(h => h.ID == id);
                return View(apiResult ?? new PT_SITEINFO());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSiteInfo(PT_SITEINFO model)
        {
            if (model == null || string.IsNullOrEmpty(model.NAME)) return Json(ApiBackModel.getRender(-1, "Name不能为空"));
            if (model.ID <= 0)
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_SITEINFO>(h => h.NAME == model.NAME, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Name已重名"));
                var back = await _sqlSugarHelper.Default.AddAsync<PT_SITEINFO>(model);
                model.ID = back;
            }
            else
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_SITEINFO>(h => h.NAME == model.NAME && h.ID != model.ID, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Name已重名"));
                await _sqlSugarHelper.Default.UpdateAsync<PT_SITEINFO>(model);
            }

            var tUser = _admin.Get();
            CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.siteinfo, "保存站点信息成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);

            return Json(ApiBackModel.getRender(model.ID, "操作成功", model.ID));
        }

        [HttpPost]
        public async Task<IActionResult> DelSiteInfo(PT_SITEINFO model)
        {
            if (model == null || model.ID <= 0) return Json(ApiBackModel.getRender(-1, "ID不能为空"));

            var check1 = await _sqlSugarHelper.Default.DeleteAsync<PT_SITEINFO>(h => h.ID == model.ID);
            if (check1 > 0)
            {
                var tUser = _admin.Get();
                CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.siteinfo, "删除站点信息成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);

                return Json(ApiBackModel.getRender(1, "操作成功"));
            }
            else return Json(ApiBackModel.getRender(-1, "删除失败"));
        }

        #endregion siteinfo

        #region 角色

        public IActionResult Roles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RolesGet(PageParm model)
        {
            var where = ExpressionFunc.GetTrue<PT_ROLE>();
            if (!string.IsNullOrEmpty(model.key))
            {
                where = where.And(h => h.NAME.Contains(model.key) || h.EXPLAIN.Contains(model.key));
            }
            var apiResult = await _sqlSugarHelper.Default.GetPagesAsync<PT_ROLE>(model, where, h => h.ID, DbOrderEnum.Asc);
            return Json(PageHelper.GetPage(apiResult));
        }

        public async Task<IActionResult> RoleEdit(int id)
        {
            if (id <= 0)
            {
                return View(new PT_ROLE());
            }
            else
            {
                var apiResult = await _sqlSugarHelper.Default.GetModelAsync<PT_ROLE>(h => h.ID == id);
                return View(apiResult ?? new PT_ROLE());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RoleSave(PT_ROLE model)
        {
            if (model == null || string.IsNullOrEmpty(model.NAME)) return Json(ApiBackModel.getRender(-1, "Name不能为空"));
            if (model.ID <= 0)
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_ROLE>(h => h.NAME == model.NAME, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Name已重名"));
                model.CTIME = DateTime.Now;
                model.POWERS = "";
                var back = await _sqlSugarHelper.Default.AddAsync<PT_ROLE>(model);
                model.ID = back;
            }
            else
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_ROLE>(h => h.NAME == model.NAME && h.ID != model.ID, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Name已重名"));

                var check2 = await _sqlSugarHelper.Default.GetModelAsync<PT_ROLE>(h => h.ID == model.ID);
                if (check2?.ID <= 0) return Json(ApiBackModel.getRender(-1, "当前数据不存在"));
                model.POWERS = check2?.POWERS ?? "";
                await _sqlSugarHelper.Default.UpdateAsync<PT_ROLE>(model);
            }

            var tUser = _admin.Get();
            CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.adminrole, "修改校色信息成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);

            return Json(ApiBackModel.getRender(model.ID, "操作成功", model.ID));
        }

        [HttpPost]
        public async Task<IActionResult> RoleDel(PT_ROLE model)
        {
            if (model == null || model.ID <= 0) return Json(ApiBackModel.getRender(-1, "ID不能为空"));

            var check0 = await _sqlSugarHelper.Default.GetListAsync<PT_ADMIN>(h => h.ROLEID == model.ID, h => h.ID, DbOrderEnum.Asc);
            if (check0?.Count > 0) return Json(ApiBackModel.getRender(-1, "该角色正在使用中，无法删除"));
            var check1 = await _sqlSugarHelper.Default.DeleteAsync<PT_ROLE>(h => h.ID == model.ID);
            if (check1.ToInt32() > 0)
            {
                var tUser = _admin.Get();
                CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.adminrole, "删除角色成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);

                return Json(ApiBackModel.getRender(1, "操作成功"));
            }
            else return Json(ApiBackModel.getRender(-1, "删除失败"));
        }

        public async Task<IActionResult> RolePower(int id)
        {
            if (id <= 0)
            {
                return View(new PT_ROLE());
            }
            else
            {
                var apiResult = await _sqlSugarHelper.Default.GetModelAsync<PT_ROLE>(h => h.ID == id);
                return View(apiResult ?? new PT_ROLE());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RolePowerGet(int id)
        {
            if (id <= 0) return Json(ApiBackModel.getRender(-1, "id不能小于0"));
            var result1 = await _sqlSugarHelper.Default.GetModelAsync<PT_ROLE>(h => h.ID == id);
            var result2 = await _sqlSugarHelper.Default.GetListAsync<PT_TREE_MENU>();
            if (result1==null|| result2 == null) return Json(ApiBackModel.getRender(-1, "数据读取错误"));
            var list1 = result2;
            var list = getPowers(0, list1, result1.POWERS);
            return Json(ApiBackModel.getRender(1, "操作成功", list));
        }

        private List<T_TREE>? getPowers(int parentid, List<PT_TREE_MENU> list, string power)
        {
            var a = list.Where(h => h.PARENTID == parentid).OrderBy(h => h.ORDERBY);
            if (a == null) return null;
            List<PT_TREE_MENU> list2 = a.ToList();

            if (list2 != null && list2.Count > 0)
            {
                List<T_TREE> back = new List<T_TREE>();

                foreach (PT_TREE_MENU m in list2)
                {
                    var childs = getPowers(m.ID.ToInt32(), list, power);
                    back.Add(new T_TREE()
                    {
                        id = m.ID.ToString(),
                        title = m.EXPLAIN + "," + m.NAME,
                        children = childs,
                        @checked = childs == null ? AdminHelper.HasPower(m.ID.ToInt32(), power) : false
                    });
                }
                return back;
            }
            else return null;
        }

        [HttpPost]
        public async Task<IActionResult> RolePowerSave(int id, List<int> data)
        {
            string powers = "";
            if (data == null || data.Count == 0) powers = "";
            else powers = getpowers(data);

            var rInt = await _sqlSugarHelper.Default.UpdateAsync<PT_ROLE>(h => new PT_ROLE() { POWERS = powers }, h => h.ID == id);
          
            if (rInt > 0)
            {
                var tUser = _admin.Get();
                CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.adminrole, "修改角色权限成功", _sqlSugarHelper);

                return Json(ApiBackModel.getRender(1, "操作成功"));
            }
            else return Json(ApiBackModel.getRender(-1, "操作失败"));
        }

        private string getpowers(List<int> list)
        {
            var a = list.OrderByDescending(h => h).ToList();
            int[] b = new int[a[0]];
            foreach (int i in a)
            {
                b[i - 1] = 1;
            }
            return string.Join("", b);
        }

        #endregion 角色

        #region 系统缓存

        public IActionResult SystemCache()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SystemCacheGet(PageParm model)
        {
            var where = ExpressionFunc.GetTrue<PT_CACHE>();
            if (!string.IsNullOrEmpty(model.key))
            {
                where = where.And(h => h.CKey.Contains(model.key) || h.CTitle.Contains(model.key));
            }
            var apiResult = await _sqlSugarHelper.Default.GetPagesAsync<PT_CACHE>(model, where, h => h.ID, DbOrderEnum.Asc);
            return Json(PageHelper.GetPage(apiResult));
        }

        public async Task<IActionResult> SystemCacheEdit(int id)
        {
            var dbs = DbConnection.GetConnections();

            List<SelectListItem> list = dbs.Select(d => new SelectListItem { Text = d.Name, Value = d.Name }).ToList();
            ViewBag.DbName = list;
            if (id <= 0)
            {
                return View(new PT_CACHE());
            }
            else
            {
                long lid = id.ToInt64();
                var apiResult = await _sqlSugarHelper.Default.GetModelAsync<PT_CACHE>(h => h.ID == lid);
                return View(apiResult ?? new PT_CACHE());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SystemCacheSave(PT_CACHE model)
        {
            if (model == null || string.IsNullOrEmpty(model.CKey)) return Json(ApiBackModel.getRender(-1, "Name不能为空"));
            if (model.ID <= 0)
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_CACHE>(h => h.CKey == model.CKey, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Ckey已重名"));

                var back = await _sqlSugarHelper.Default.AddAsync<PT_CACHE>(model);
                model.ID = back;
            }
            else
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_CACHE>(h => h.CKey == model.CKey && h.ID != model.ID, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Ckey已重名"));

                var check2 = await _sqlSugarHelper.Default.GetModelAsync<PT_CACHE>(h => h.ID == model.ID);
                if (check2?.ID <= 0) return Json(ApiBackModel.getRender(-1, "当前数据不存在"));

                var rt3 = await _sqlSugarHelper.Default.UpdateAsync<PT_CACHE>(model);
                model.ID = rt3 > 0 ? model.ID : -1;
            }
            if (model.ID > 0)
            {
                Cx.Redis.ProjectCache.Default.ReLoad_Setting();
                CAdmin.Log("admin", "admin", E_AminLog.systemcacheset, "修改缓存配置", _sqlSugarHelper);
            }
            return Json(ApiBackModel.getRender(model.ID, "操作成功", model.ID));
        }

        [HttpPost]
        public async Task<IActionResult> SystemCacheDel(PT_CACHE model)
        {
            var check1 = await _sqlSugarHelper.Default.DeleteAsync<PT_CACHE>(h => h.ID == model.ID);
            int rInt = check1;
            if (rInt <= 0) return Json(ApiBackModel.getRender(-1, "删除失败"));
            Cx.Redis.ProjectCache.Default.ReLoad_Setting();
            CAdmin.Log("admin", "admin", E_AminLog.systemcacheset, "删除缓存配置", _sqlSugarHelper);
            return Json(ApiBackModel.getRender(model.ID, "操作成功"));
        }

        #endregion 系统缓存

        #region 管理员

        public IActionResult Admins()
        {
            DataTable? ht = ProjectCache.Default.GetDataTable(AdminConst.Http_Role).Result;
            List<SelectListItem> list = new List<SelectListItem>();
            if (ht != null)
            {
                foreach (DataRow dr in ht.Rows)
                {
                    list.Add(new SelectListItem() { Text = dr["EXPLAIN"].ToString(), Value = dr["ID"].ToString() });
                }
            }
            ViewBag.SelectRoles = list;
            return View();
        }

        public async Task<IActionResult> AdminGet(SearchPagePam model)
        {
            var where = ExpressionFunc.GetTrue<PT_ADMIN>();
            if (!string.IsNullOrEmpty(model.key))
            {
                where = where.And(h => h.UNAME.Contains(model.key) || h.REALNAME.Contains(model.key));
            }
            if (model.roleid > 0)
            {
                where = where.And(h => h.ROLEID.Equals(model.roleid));
            }
            //var apiResult = await _sqlSugarHelper.Default.GetPagesAsync<PT_ADMIN>(model, where, h => h.ID, DbOrderEnum.Asc);
            var pagelist = await _sqlSugarHelper.Default!.Queryable<PT_ADMIN>().InnerJoin<PT_ROLE>((a, b) => a.ROLEID == b.ID).Where(where).Select((a, b) => new PT_ADMIN_PLUS() { ID = a.ID, ROLEID = a.ROLEID, REALNAME = a.REALNAME, ROLENAME = b.EXPLAIN, UNAME = a.UNAME, USTATE = a.USTATE, CTIME = a.CTIME }).ToPageAsync(model.page, model.limit);

            return Json(PageHelper.GetPage(pagelist));
        }

        public async Task<IActionResult> AdminEdit(int id)
        {
            DataTable? ht = ProjectCache.Default.GetDataTable(AdminConst.Http_Role).Result;
            List<SelectListItem> list = new List<SelectListItem>();
            if (ht != null)
            {
                foreach (DataRow dr in ht.Rows)
                {
                    list.Add(new SelectListItem() { Text = dr["EXPLAIN"].ToString(), Value = dr["ID"].ToString() });
                }
            }
            ViewBag.SelectRoles = list;

            List<SelectListItem> list2 = new List<SelectListItem>();

            var ListKeys = EnumDescriptionExtension.GetFields(typeof(E_USTATE));
            if (ListKeys != null && ListKeys.Count > 0)
            {
                foreach (var dr in ListKeys)
                {
                    list2.Add(new SelectListItem() { Text = dr.Value.ToString(), Value = dr.Key.ToString() });
                }
            }
            ViewBag.UState = list2;

            if (id <= 0)
            {
                return View(new PT_ADMIN());
            }
            else
            {
                long lid = id.ToInt64();
                var apiResult = await _sqlSugarHelper.Default.GetModelAsync<PT_ADMIN>(h => h.ID == lid);
                return View(apiResult ?? new PT_ADMIN());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdminSave(PT_ADMIN model)
        {
            if (model == null || string.IsNullOrEmpty(model.UNAME)) return Json(ApiBackModel.getRender(-1, "Name不能为空"));
            if (model.ID <= 0)
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_ADMIN>(h => h.UNAME == model.UNAME, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Ckey已重名"));
                model.PWD = DataUtils.MD5(model.PWD);
                var back = await _sqlSugarHelper.Default.AddAsync<PT_ADMIN>(model);
                model.ID = back;
            }
            else
            {
                var check1 = await _sqlSugarHelper.Default.GetListAsync<PT_ADMIN>(h => h.UNAME == model.UNAME && h.ID != model.ID, h => h.ID, DbOrderEnum.Asc);
                if (check1?.Count > 0) return Json(ApiBackModel.getRender(-1, "Ckey已重名"));
                if (!string.IsNullOrEmpty(model.PWD)) model.PWD = DataUtils.MD5(model.PWD);
                var rt3 = await _sqlSugarHelper.Default.UpdateAsync<PT_ADMIN>(model);
                model.ID = rt3> 0 ? model.ID : -1;
            }
            if (model.ID > 0)
            {
                var tUser = _admin.Get();
                CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.editadmin, "修改管理员信息成功(" + JsonConvert.SerializeObject(model) + ")", _sqlSugarHelper);
            }
            return Json(ApiBackModel.getRender(model.ID, "操作成功", model.ID));
        }

        [HttpPost]
        public async Task<IActionResult> AdminDel(int id, int tostate)
        {
            var rInt = await _sqlSugarHelper.Default.UpdateAsync<PT_ADMIN>(h => new PT_ADMIN { USTATE = tostate }, h => h.ID == id);

            if (rInt <= 0) return Json(ApiBackModel.getRender(-1, "操作失败"));
            var tUser = _admin.Get();
            CAdmin.Log(tUser.UNAME, tUser.REALNAME, E_AminLog.editadmin, "修改管理员状态为(" + EnumDescriptionExtension.GetEnumDescription(typeof(E_USTATE), tostate) + ")", _sqlSugarHelper);
            return Json(ApiBackModel.getRender(1, "操作成功"));
        }

        #endregion 管理员

        #region 修改密码

        public IActionResult MyPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMyPassword(string oldpwd, string newpwd)
        {
            var tadmin = _admin.Get();
            var oldpwd_md5 = Cx.Data.Md5Crypt.Encrypt(oldpwd);
            var ck_admin = await _sqlSugarHelper.Default.GetModelAsync<PT_ADMIN>(h => h.UNAME == tadmin.UNAME && h.PWD == oldpwd_md5);
            if (ck_admin== null) return Json(ApiBackModel.getRender(-1, "原密码错误"));
            ck_admin.PWD = Cx.Data.Md5Crypt.Encrypt(newpwd);
            var b = await _sqlSugarHelper.Default.UpdateAsync<PT_ADMIN>(ck_admin);
            return Json(ApiBackModel.getRender(200, "操作成功"));
        }

        #endregion

        #region 修改头像
        public IActionResult Avatar()
        {
            var tadmin = _admin.Get();
            return View(tadmin);
        }

        #endregion
    }
}