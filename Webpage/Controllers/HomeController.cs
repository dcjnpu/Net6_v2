using BaseBLL;
using Cx.Data;
using Cx.EPPlus;
using Cx.Redis;
using Cx.SqlSugarV2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Webpage.Models;

namespace Webpage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPubSugar _sqlSugarHelper;
        private readonly IProjectCache _projectCache;
        // private readonly IAdmin _admin;
        private readonly IWebHostEnvironment _ev;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Admin_MyJwt _admin;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, Admin_MyJwt admin, IPubSugar sqlSugarHelper, IProjectCache projectCache, IWebHostEnvironment ev, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _logger = logger;
            _sqlSugarHelper=sqlSugarHelper;
            _projectCache=projectCache;
            _admin=admin;
            _ev=ev;
            this.httpContextAccessor=httpContextAccessor!;
            this.configuration=configuration;
        }

        #region 菜单

        private string BindMenuTop0()
        {
            //<li data-name="set" class="layui-nav-item">
            //    <a href="javascript:;" lay-tips="设置" lay-direction="2">
            //        <i class="layui-icon layui-icon-set"></i>
            //        <cite>设置</cite>
            //    </a>
            //    <dl class="layui-nav-child">
            //        <dd class="layui-nav-itemed">
            //            <a href="javascript:;">系统设置</a>
            //            <dl class="layui-nav-child">
            //                <dd><a lay-href="set/system/website.html">网站设置</a></dd>
            //                <dd><a lay-href="set/system/email.html">邮件服务</a></dd>
            //            </dl>
            //        </dd>
            //        <dd class="layui-nav-itemed">
            //            <a href="javascript:;">我的设置</a>
            //            <dl class="layui-nav-child">
            //                <dd><a lay-href="set/user/info.html">基本资料</a></dd>
            //                <dd><a lay-href="set/user/password.html">修改密码</a></dd>
            //            </dl>
            //        </dd>
            //    </dl>
            //</li>
            //<li data-name="get" class="layui-nav-item">
            //    <a href="javascript:;" lay-href="//www.layui.com/admin/#get" lay-tips="授权" lay-direction="2">
            //        <i class="layui-icon layui-icon-auz"></i>
            //        <cite>授权</cite>
            //    </a>
            //</li>

            DataTable? dt = _projectCache.GetDataTable(AdminConst.Http_AdminMenu).Result;
            if (dt==null||dt.Rows.Count==0) return String.Empty;

            TAdmin2 tadmin = _admin.Get();

            DataRow[] drs = dt.Select("PARENTID=0");
            StringBuilder sb = new StringBuilder();
            int id = 0; string url = string.Empty; string name = string.Empty; string explain = string.Empty; string mclass = string.Empty;
            for (int i = 0; i < drs.Length; i++)
            {
                id = drs[i]["ID"].ToInt32();
                url = geturl(drs[i]["VALUE"].ToString()??"");
                explain = drs[i]["EXPLAIN"].ToString()??"";
                name = drs[i]["NAME"].ToString()??"";
                mclass = drs[i]["CLASS"].ToString()??"";

                if (!AdminHelper.HasPower(id, tadmin.POWER)) continue;
                string child = GetChild1(dt, id, tadmin);
                if (string.IsNullOrEmpty(child))
                {
                    if (!string.IsNullOrEmpty(url))
                        sb.AppendFormat("<li data-name=\"{2}\" class=\"layui-nav-item\"><a href=\"javascript:;\" lay-href=\"{0}\" lay-tips=\"{1}\" lay-direction=\"2\"><i class=\"layui-icon {3}\"></i><cite>{1}</cite></a></li>", url, explain, name, mclass);
                }
                else sb.AppendFormat("<li data-name=\"{2}\" class=\"layui-nav-item\"><a href=\"javascript:;\" lay-tips=\"{1}\" lay-direction=\"2\"><i class=\"layui-icon {3}\"></i><cite>{1}</cite></a>{0}</li>", child, explain, name, mclass);
            }
            return sb.ToString();
        }

        private string GetChild1(DataTable dt, int pid, TAdmin tadmin)
        {
            //<dl class="layui-nav-child">
            //                            <dd data-name="list"><a lay-href="component/grid/list.html">等比例列表排列</a></dd>
            //                            <dd data-name="mobile"><a lay-href="component/grid/mobile.html">按移动端排列</a></dd>
            //                            <dd data-name="mobile-pc"><a lay-href="component/grid/mobile-pc.html">移动桌面端组合</a></dd>
            //                            <dd data-name="all"><a lay-href="component/grid/all.html">全端复杂组合</a></dd>
            //                            <dd data-name="stack"><a lay-href="component/grid/stack.html">低于桌面堆叠排列</a></dd>
            //                            <dd data-name="speed-dial"><a lay-href="component/grid/speed-dial.html">九宫格</a></dd>
            //                        </dl>
            DataRow[] drs = dt.Select("PARENTID=" + pid);
            StringBuilder sb = new StringBuilder();
            int id = 0; string url = string.Empty; string name = string.Empty; string explain = string.Empty;
            string child = string.Empty;
            for (int i = 0; i < drs.Length; i++)
            {
                id = drs[i]["ID"].ToInt32();
                url = geturl(drs[i]["VALUE"].ToString()??"");
                explain = drs[i]["EXPLAIN"].ToString()??"";
                name = drs[i]["NAME"].ToString()??"";
                if (!AdminHelper.HasPower(id, tadmin.POWER)) continue;

                child = GetChild1(dt, id, tadmin);
                if (string.IsNullOrEmpty(child))
                {
                    if (!string.IsNullOrEmpty(url))
                        sb.AppendFormat("<dd data-name=\"{2}\"><a lay-href=\"{0}\">{1}</a></dd>", url, explain, name);
                }
                else
                    sb.AppendFormat("<dd data-name=\"{2}\"><a href='javascript:;'>{1}</a>{0}</dd>", child, explain, name);
            }
            string childs = sb.ToString();
            if (string.IsNullOrEmpty(childs)) return string.Empty;
            return "<dl class=\"layui-nav-child\">" + childs + "</dl>";
        }

        private string geturl(string url)
        {
            if (url.IndexOf("~") == 0) return url.Substring(1);
            return string.Empty;
        }

        #endregion 菜单

        [Authorize]
        public IActionResult Index()
        {


            ViewBag.Menu=BindMenuTop0();
            var tadmin = _admin.Get();
            return View(tadmin);
        }

        //public IActionResult Export()
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    Cx.EPPlus.ExcelHelper.Export(dt, (a =>
        //    {
        //        a.SaveAs(memoryStream);
        //        //转化为byte[]

        //        memoryStream.Seek(0, SeekOrigin.Begin);//没这句话就格式错
        //    }), "tt");
        //    byte[] bytes = memoryStream.ToArray();
        //    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //}
        public IActionResult Export()
        {
            DataTable dt = ExcelHelper.getData();
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\3.xlsx");
            var path0 = "excelfile\\" + Guid.NewGuid().ToString() + ".xlsx";
            var path = Path.Combine(_ev.WebRootPath, path0);
            FileInfo file = new FileInfo(path);
            if (file.Exists) file.Delete();
            ExcelHelper.Export(dt, (a =>
            {
                a.SaveAs(file);
            }), "tt");
            Task.Run(() =>
            {
                Thread.Sleep(10000);
                file.Delete();
            });
            return File(path0, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "3.xlsx");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            //var cc = CxHttpContextExtensions.Current;
            //var ip = cc.IP();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string u, string p, string c)
        {
            ApiResult<PT_ADMIN>? model = null;
            //byte[] out1 = null;
            bool flag = HttpContext.Session.TryGetValue("logincode", out byte[]? bytes);
            if (bytes == null) { model = new ApiResult<PT_ADMIN>() { message = "请刷新验证码后重新验证", statusCode = -201 }; return Json(model); }
            var code = Encoding.Default.GetString(bytes);
            if (!code.Equals(c.ToLower())) { model = new ApiResult<PT_ADMIN>() { message = "验证码错误", statusCode = -201 }; return Json(model); }

            if (p.Length != 32) p = DataUtils.MD5(p);
            var model1 = await _sqlSugarHelper.Default.GetModelAsync<PT_ADMIN>(h => h.UNAME == u && h.PWD == p);

            if (model1!=null)
            {
                var tadmin = model1; model = new ApiResult<PT_ADMIN>();
                if (string.IsNullOrEmpty(tadmin.UNAME)) { model.statusCode = (int)ApiEnum.OprateFail; model.message = "帐号或密码错误"; }
                else if (tadmin.USTATE != (int)E_USTATE.Normal) { model.statusCode = (int)ApiEnum.OprateFail; model.message = "该账号已禁止登录"; }
                else
                {
                    TAdmin2 tadmin2 = new TAdmin2()
                    {
                        REALNAME=tadmin.REALNAME,
                        ROLEID=   tadmin.ROLEID,
                        UNAME=   tadmin.UNAME,
                        USTATE= tadmin.USTATE,
                        GUID= tadmin.UNAME
                    };
                    model.message = "登录成功";
                    var ht = _projectCache.GetHashTable(AdminConst.Http_Role).Result;
                    if (ht!=null)
                    {
                        tadmin2.POWER = ht[tadmin.ROLEID.ToString()]?.ToString()??"";// ht[tadmin.ROLEID.ToString()].ToString()??"";
                    }

                    CAdmin.Log(tadmin2.UNAME, tadmin2.REALNAME, E_AminLog.login, "登陆成功", _sqlSugarHelper);

                    _admin.Set(tadmin2);
                    base.HttpContext.Response.Cookies.Append(configuration["JWT:CookieKey"], tadmin2.Token);
                    //var user = HttpContext.User;
                }
            }
            return Json(model);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginOut()
        {
            var tadmin2 = _admin.Get();
            _admin.LoginOut();
            base.HttpContext.Response.Cookies.Delete(configuration["JWT:CookieKey"]);
            CAdmin.Log(tadmin2.UNAME, tadmin2.REALNAME, E_AminLog.login, "登陆成功", _sqlSugarHelper);
            ApiResult<String> model = new ApiResult<string>() { statusCode = (int)ApiEnum.Status, message = "登出成功" };
            return Json(model);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        public IActionResult Captcha([FromServices] IImageCaptcha imageCaptcha)
        {
            var code = Cx.Data.DataUtils.GetCheckCode(4).ToLower();
            HttpContext.Session.Set("logincode", Encoding.Default.GetBytes(code));
            var bytes = imageCaptcha.GetCaptcha(code, 110, 32);
            return File(bytes, "image/png");
        }
    }
}