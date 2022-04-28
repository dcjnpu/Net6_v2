using BaseBLL;
using Cx.Data;
using Cx.Redis;
using Cx.SqlSugarV2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IPubSugar _sqlSugarHelper;
        private readonly IProjectCache _projectCache;
        private readonly IAdmin _admin;

        public AdminController(IAdmin admin, IPubSugar sqlSugarHelper, IProjectCache projectCache)
        {
            _sqlSugarHelper=sqlSugarHelper;
            _projectCache=projectCache;
            _admin=admin;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<TApi_Result_Data<TAdmin>> Login(string u = "admin", string p = "123456")
        {
            TApi_Result_Data<TAdmin> rt = new TApi_Result_Data<TAdmin>();
            var result = await _sqlSugarHelper.Default!.GetModelAsync<PT_ADMIN>(h => h.UNAME==u&&h.PWD==Cx.Data.Md5Crypt.Encrypt(p));
            if (result!=null)
            {
                rt.data.UNAME=result.UNAME;
                rt.data.REALNAME=result.REALNAME;
                rt.data.CTIME=DateTime.Now;
                _admin.Set(rt.data);
            }
            else
            {
                rt.code=(int)ApiEnum.Error;
                rt.message=EnumDescriptionExtension.GetEnumDescription(typeof(ApiEnum), rt.code);
            }
            return rt;
        }

        [HttpPost]
        public async Task<TApi_Result> EditMyPassWord(string oldpwd = "123456", string newpwd = "654321")
        {
            TApi_Result rt = new TApi_Result();
            var tadmin = _admin.Get();
            //var a = base.User.FindFirst("");
            var oldpwd_md5 = Cx.Data.Md5Crypt.Encrypt(oldpwd);
            var ck_admin = await _sqlSugarHelper.Default.GetModelAsync<PT_ADMIN>(h => h.UNAME==tadmin.UNAME&&h.PWD==oldpwd_md5);

            if (ck_admin==null) { return new TApi_Result() { code=-301, message="原密码错误" }; }

            ck_admin.PWD= Cx.Data.Md5Crypt.Encrypt(newpwd);
            var b = await _sqlSugarHelper.Default.UpdateAsync<PT_ADMIN>(ck_admin);

            return new TApi_Result() { message="修改密码成功" };
        }
    }
}