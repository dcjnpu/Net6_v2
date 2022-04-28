using Cx.Data;
using Cx.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Cx.Jwt
{
    /// <summary>
    /// JWT认证 方法
    /// 基本配置 JWT下：Expires 过期时间，UseRedis：是否使用Redis缓存用户数据，SecretKey：密钥，Issuer，Audience，RedisHeader：缓存主键头部
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoginToken_Jwt<T> : IAdmin<T> where T : IToken, new()
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        List<PropertyInfo> properties = new List<PropertyInfo>();
        public LoginToken_Jwt(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            SetJwtKeys();
        }



        void SetJwtKeys()
        {
            properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(h => h.CanRead && AttributeHelper.ContainMyAttribute(h, typeof(JwtKeyAttribute))).ToList();
            if (properties == null || properties.Count == 0) return;
        }

        //private Type _type_datetime = typeof(DateTime);
        public void Set(T tadmin)
        {
            int expireHours = _configuration["JWT:Expires"].ToInt32(1);
            bool IsUserRedis = _configuration["JWT:UseRedis"].Equals("true", StringComparison.OrdinalIgnoreCase);
            var claims = new List<Claim>();



            properties.ForEach(h =>
               {
                   var obj = h.GetValue(tadmin, null);
                   if (obj != null) claims.Add(new Claim(h.Name, obj.ToString() ?? ""));
               });



            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 6. 根据以上，生成token
            var token = new JwtSecurityToken(
                 _configuration["JWT:Issuer"],     //Issuer
                 _configuration["JWT:Audience"],   //Audience
                 claims,                          //Claims,
                 DateTime.Now,                    //notBefore
                 DateTime.Now.AddHours(expireHours),   //expires
                 signingCredentials               //Credentials
             );

            // 7. 将token变为string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (IsUserRedis)
            {
                string redis_head = _configuration["JWT:RedisHeader"];
                Task.Run(() =>
                    {
                        int expireSecond = 60 * 60 * expireHours;
                        Caching.Default.Redis.Set(CacheHelper.GetKey(redis_head + "_" + tadmin.GUID, Cx.Redis.E_CacheDataType.character), tadmin, expireSecond);
                    });
            }

            tadmin.Token = jwtToken;

        }

        /// <summary>
        /// get其实没什么用,如果使用redis缓存用户信息，则主要数据用这个获取
        /// </summary>
        /// <returns></returns>
        public T Get2()
        {
            //httpContextAccessor.HttpContext.
            //string token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"] ?? "";
            //从cookie中获取token
            bool flag = httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("logintoken", out string? token);

            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ")) return new T();
            //context
            token = token.Substring("Bearer ".Length).Trim();


            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);

            bool IsUserRedis = _configuration["JWT:UseRedis"].Equals("true", StringComparison.OrdinalIgnoreCase);
            string redis_head = _configuration["JWT:RedisHeader"];

            if (IsUserRedis)
            {
                jwtToken.Payload.TryGetValue("GUID", out object? uname);
                return Caching.Default.Redis.Get<T>(CacheHelper.GetKey(redis_head + "_" + uname?.ToString() ?? "", Cx.Redis.E_CacheDataType.character));
            }
            T Admin = new T();
            properties.ForEach(h =>
            {
                jwtToken.Payload.TryGetValue(h.Name, out object? value);
                //var value = User.FindFirstValue(h.Name);
                var val = Convert.ChangeType(value, h.PropertyType);
                h.SetValue(Admin, val, null);
            });
            return Admin;
        }

        /// <summary>
        /// 转成T的格式
        /// </summary>
        /// <returns></returns>
        public T GetSingle(HttpContext? httpContext = null)
        {
            if (httpContext == null) httpContext = httpContextAccessor.HttpContext;
            var User = httpContext!.User;
            if (User == null) return new T();
            T Admin = new T();
            properties.ForEach(h =>
            {
                var value = User.FindFirstValue(h.Name);
                if (value!=null)
                {
                    try
                    {
                        var t = Nullable.GetUnderlyingType(h.PropertyType) ?? h.PropertyType;
                        var val = Convert.ChangeType(value, t);
                        h.SetValue(Admin, val, null);
                    }
                    catch { }
                }
            });
            return Admin;
        }

        /// <summary>
        /// 转成T的格式
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            var User = httpContextAccessor.HttpContext!.User;
            if (User == null) return new T();
            bool IsUserRedis = _configuration["JWT:UseRedis"].Equals("true", StringComparison.OrdinalIgnoreCase);
            string redis_head = _configuration["JWT:RedisHeader"];

            if (IsUserRedis)
            {
                var uname = User.FindFirstValue("GUID");
                return Caching.Default.Redis.Get<T>(CacheHelper.GetKey(redis_head + "_" + uname?.ToString() ?? "", Cx.Redis.E_CacheDataType.character));
            }
            T Admin = new T();

            properties.ForEach(h =>
            {
                var value = User.FindFirstValue(h.Name);
                if (value!=null)
                {
                    try
                    {
                        var t = Nullable.GetUnderlyingType(h.PropertyType) ?? h.PropertyType;
                        var val = Convert.ChangeType(value, t);
                        h.SetValue(Admin, val, null);
                    }
                    catch { }
                }
            });
            return Admin;
        }


        public bool IsLogin()
        {
            T Admin = Get();
            if (string.IsNullOrEmpty(Admin.GUID)) return false;
            return true;
        }


        public void LoginOut()
        {
            var User = httpContextAccessor.HttpContext?.User;
            if (User == null) return;
            var uname = User.FindFirstValue("GUID");
            if (string.IsNullOrEmpty(uname)) return;

            bool IsUserRedis = _configuration["JWT:UseRedis"].Equals("true", StringComparison.OrdinalIgnoreCase);
            string redis_head = _configuration["JWT:RedisHeader"];

            if (IsUserRedis)
            {
                Caching.Default.Redis.DelAsync(CacheHelper.GetKey(redis_head + "_" + uname, Cx.Redis.E_CacheDataType.character));
            }
        }


    }
}