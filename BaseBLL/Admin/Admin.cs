using Cx.Data;
using Cx.Redis;
using Cx.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;

namespace BaseBLL
{
    public class TAdmin : PT_ADMIN
    {
        [JwtKey]
        public string POWER { get; set; } = "";
    }
    public class TAdmin2 : TAdmin, IToken
    {
       // public string POWER { get; set; } = "";
        private string _token = string.Empty;
        private string _guid = string.Empty;
        [JwtKey]
        public string GUID { get => _guid; set => _guid=value; }
        public string Token { get => _token; set => _token=value; }

        //public string POWER { get; set; } = string.Empty;
    }
    public class Admin_MyJwt : LoginToken_Jwt<TAdmin2>
    {
        public Admin_MyJwt(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
            //this.admin=admin;
        }
    }

    //public class Admin_Cookie : IAdmin
    //{
    //    //private static readonly string LoginCookieKey = ConfigExtensions.AppSettings["keys:cookiename"];

    //    private bool IsUseRedis = false;

    //    private string redis_head = "iadmin";

    //    private readonly HttpContext context;

    //    public Admin_Cookie(IHttpContextAccessor httpContextAccessor)
    //    {
    //        context=((HttpContextAccessor)httpContextAccessor).HttpContext;
    //    }

    //    public TAdmin Get()
    //    {
    //        var auth = context.AuthenticateAsync().Result;

    //        var authinfo = auth.Principal.Identities.First(u => u.IsAuthenticated);
    //        var userGuid = authinfo.FindFirst(ClaimTypes.Sid)!.Value;

    //        if (string.IsNullOrEmpty(userGuid)) return new TAdmin();
    //        if (IsUseRedis)
    //            return Caching.Default.Redis.Get<TAdmin>(CacheHelper.GetKey(redis_head+"_"+userGuid, Cx.Redis.E_CacheDataType.character));
    //        else
    //        {
    //            TAdmin rt = new TAdmin();
    //            rt.UNAME=userGuid;
    //            rt.REALNAME=authinfo.FindFirst(ClaimTypes.Name)!.Value;
    //            rt.ROLEID=authinfo.FindFirst(ClaimTypes.Role)!.Value.ToInt32();
    //            rt.POWER=authinfo.FindFirst("POWER")?.Value??"";
    //            return rt;
    //        }
    //    }

    //    public void Set(TAdmin tadmin, int expireHours = 8)
    //    {
    //        var claims = new List<Claim>()//身份验证信息
    //                {
    //                    new Claim(ClaimTypes.Sid,tadmin.UNAME),
    //                    new Claim(ClaimTypes.Name,tadmin.REALNAME),
    //                    new Claim(ClaimTypes.Role,tadmin.ROLEID.ToString()),
    //                    new Claim("POWER",tadmin.POWER.ToString())
    //                };

    //        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
    //        context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
    //        {
    //            ExpiresUtc = DateTime.UtcNow.AddHours(expireHours),//过期时间：30分钟
    //        }).Wait();

    //        int expireSecond = 60 * 60  * expireHours;
    //        string loginkey = Guid.NewGuid().ToString();
    //        if (IsUseRedis)
    //            Caching.Default.Redis.Set(CacheHelper.GetKey(redis_head+"_"+tadmin.UNAME, Cx.Redis.E_CacheDataType.character), tadmin, expireSecond);
    //    }

    //    public void LoginOut()
    //    {
    //        context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //    }

    //    /// <summary>
    //    /// 判断是否为登录状态
    //    /// </summary>
    //    /// <param name="current"></param>
    //    /// <returns></returns>
    //    public bool IsLogin()
    //    {
    //        var auth = context.AuthenticateAsync().Result;

    //        var authinfo = auth.Principal.Identities.First(u => u.IsAuthenticated);
    //        return authinfo!=null;
    //    }
    //}

    //public class Admin_Cookie_Jwt : IAdmin
    //{
    //    private static readonly string LoginCookieKey = ConfigExtensions.AppSettings["keys:cookiename"];

    //    private readonly HttpContext current;

    //    public Admin_Cookie_Jwt(IHttpContextAccessor httpContextAccessor)
    //    {
    //        current=((HttpContextAccessor)httpContextAccessor).HttpContext;
    //    }

    //    /// <summary>
    //    /// 获取登录信息
    //    /// </summary>
    //    /// <param name="current"></param>
    //    /// <returns></returns>
    //    public TAdmin Get()
    //    {
    //        bool flag = current.Request.Cookies.TryGetValue(JwtHelper.key_token, out string? tokencookie);
    //        //context
    //        if (flag)
    //        {
    //            //tokencookie = tokencookie.ToString().Substring("Bearer ".Length).Trim();
    //            var jwtHandler = new JwtSecurityTokenHandler();
    //            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(tokencookie);

    //            jwtToken.Payload.TryGetValue(ClaimTypes.Sid, out object? tokenvalue);
    //            if (tokenvalue == null) return new TAdmin();
    //            return Caching.Default.Redis.Get<TAdmin>(CacheHelper.GetKey(tokenvalue.ToString() ?? "", Cx.Redis.E_CacheDataType.character));
    //        }
    //        return new TAdmin();
    //    }

    //    /// <summary>
    //    /// 设置登录人信息
    //    /// </summary>
    //    /// <param name="tadmin"></param>
    //    /// <param name="expireDay"></param>
    //    /// <param name="current"></param>
    //    public void Set(TAdmin tadmin, int expireHours)
    //    {
    //        //if (current == null) current = CxHttpContext.Current;
    //        string loginkey = Guid.NewGuid().ToString();
    //        int expireDay = Convert.ToInt32(ConfigExtensions.AppSettings["JWT:Expires"]);

    //        var task1 = JwtHelper.SetToken(loginkey, expireDay);

    //        var task2 = Task.Run(() =>
    //        {
    //            int expireSecond = 60 * 60 * 24 * expireDay;
    //            Caching.Default.Redis.Set(CacheHelper.GetKey(loginkey, Cx.Redis.E_CacheDataType.character), tadmin, expireSecond);
    //        });
    //        string tokencookie = task1.Result;
    //        current.Response.Cookies.Append(JwtHelper.key_token, tokencookie);
    //    }

    //    /// <summary>
    //    /// 判断是否为登录状态
    //    /// </summary>
    //    /// <param name="current"></param>
    //    /// <returns></returns>
    //    public bool IsLogin()
    //    {
    //        bool flag = current.Request.Cookies.TryGetValue(JwtHelper.key_token, out string? tokencookie);
    //        if (!flag || string.IsNullOrEmpty(tokencookie)) return false;
    //        return true;
    //    }

    //    public void LoginOut()
    //    {
    //        bool flag = current.Request.Cookies.TryGetValue(JwtHelper.key_token, out string? tokencookie);
    //        if (flag)
    //        {
    //            //取出jwt中的Sid
    //            var jwtHandler = new JwtSecurityTokenHandler();
    //            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(tokencookie);
    //            jwtToken.Payload.TryGetValue(ClaimTypes.Sid, out object? tokenvalue);

    //            if (tokenvalue != null)
    //            {
    //                Caching.Default.Redis.Del(CacheHelper.GetKey(tokenvalue.ToString() ?? "", Cx.Redis.E_CacheDataType.character));
    //                current.Response.Cookies.Delete(JwtHelper.key_token);
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// jwt配合redis,header
    ///// </summary>
    //public class Admin_Header_Jwt : IAdmin
    //{
    //    // private static readonly string LoginCookieKey = ConfigExtensions.AppSettings["keys:cookiename"];
    //    private string redis_head = "iadmin2";

    //    private bool IsUseRedis = false;

    //    private readonly HttpContext current;

    //    public Admin_Header_Jwt(IHttpContextAccessor httpContextAccessor)
    //    {
    //        current=((HttpContextAccessor)httpContextAccessor).HttpContext!;
    //    }

    //    /// <summary>
    //    /// 获取登录信息
    //    /// </summary>
    //    /// <param name="current"></param>
    //    /// <returns></returns>
    //    public TAdmin Get()
    //    {
    //        //context
    //        if (current.Request.Headers.ContainsKey("Authorization"))
    //        {
    //            var tokenHeader = current.Request.Headers["Authorization"];
    //            if (tokenHeader.ToString().StartsWith("Bearer "))
    //                tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();

    //            var jwtHandler = new JwtSecurityTokenHandler();
    //            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(tokenHeader);

    //            jwtToken.Payload.TryGetValue(ClaimTypes.Sid, out object? uname);

    //            if (uname == null) return new TAdmin();
    //            if (IsUseRedis)
    //            {
    //                return Caching.Default.Redis.Get<TAdmin>(CacheHelper.GetKey(redis_head+"_"+uname.ToString(), Cx.Redis.E_CacheDataType.character));
    //            }

    //            //jwtToken.Payload.TryGetValue(ClaimTypes.Sid, out object? uname);
    //            jwtToken.Payload.TryGetValue(ClaimTypes.Name, out object? realname);

    //            return new TAdmin()
    //            {
    //                UNAME=uname?.ToString()??"",
    //                REALNAME=realname?.ToString()??""
    //            };
    //        }
    //        return new TAdmin();
    //    }

    //    /// <summary>
    //    /// 设置登录人信息
    //    /// </summary>
    //    /// <param name="tadmin"></param>
    //    /// <param name="expireDay"></param>
    //    /// <param name="current"></param>
    //    public void Set(TAdmin tadmin, int expireHours)
    //    {
    //        //string loginkey = Guid.NewGuid().ToString();
    //        int expireDay = Convert.ToInt32(ConfigExtensions.AppSettings["JWT:Expires"]);

    //        var claims = new[]
    //           {
    //                new Claim(ClaimTypes.Sid, tadmin.UNAME),
    //                new Claim(ClaimTypes.Name,tadmin.REALNAME),
    //           };

    //        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigExtensions.AppSettings["JWT:SecretKey"]));
    //        // 3. 选择加密算法
    //        var algorithm = SecurityAlgorithms.HmacSha256;

    //        // 4. 生成Credentials
    //        var signingCredentials = new SigningCredentials(secretKey, algorithm);

    //        // 6. 根据以上，生成token
    //        var token = new JwtSecurityToken(
    //             ConfigExtensions.AppSettings["JWT:Issuer"],     //Issuer
    //             ConfigExtensions.AppSettings["JWT:Audience"],   //Audience
    //             claims,                          //Claims,
    //             DateTime.Now,                    //notBefore
    //             DateTime.Now.AddDays(expireDay),   //expires
    //             signingCredentials               //Credentials
    //         );

    //        // 7. 将token变为string
    //        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

    //        if (IsUseRedis)
    //            Task.Run(() =>
    //            {
    //                int expireSecond = 60 * 60 * 24 * expireDay;
    //                Caching.Default.Redis.Set(CacheHelper.GetKey(redis_head+"_"+tadmin.UNAME, Cx.Redis.E_CacheDataType.character), tadmin, expireSecond);
    //            });

    //        tadmin.POWER=jwtToken;
    //    }

    //    /// <summary>
    //    /// 判断是否为登录状态
    //    /// </summary>
    //    /// <param name="current"></param>
    //    /// <returns></returns>
    //    public bool IsLogin()
    //    {
    //        if (current.Request.Headers.ContainsKey("Authorization"))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public void LoginOut()
    //    {
    //    }
    //}

    //public class JwtHelper
    //{
    //    //public const string key_token = "logintoken";

    //    public static Task<string> SetToken(string loginkey, int expireDay)
    //    {
    //        var task1 = Task.Run(() =>
    //        {
    //            var claims = new[]
    //            {
    //                new Claim(ClaimTypes.Sid, loginkey),
    //             };

    //            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigExtensions.AppSettings["JWT:SecretKey"]));

    //            // 3. 选择加密算法
    //            var algorithm = SecurityAlgorithms.HmacSha256;

    //            // 4. 生成Credentials
    //            var signingCredentials = new SigningCredentials(secretKey, algorithm);

    //            // 6. 根据以上，生成token
    //            var token = new JwtSecurityToken(
    //                 ConfigExtensions.AppSettings["JWT:Issuer"],     //Issuer
    //                 ConfigExtensions.AppSettings["JWT:Audience"],   //Audience
    //                 claims,                          //Claims,
    //                 DateTime.Now,                    //notBefore
    //                 DateTime.Now.AddDays(expireDay),   //expires
    //                 signingCredentials               //Credentials
    //             );

    //            // 7. 将token变为string
    //            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
    //            //current.Response.Cookies.Append(LoginCookieKey, jwtToken);
    //            //current.Response.Headers.Append("Authentication", "Bearer " + jwtToken);
    //            return jwtToken;
    //        });
    //        return task1;
    //    }
    //}
}