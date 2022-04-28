using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Cx.Jwt
{
    public interface IAdmin<T> where T : IToken
    {
        /// <summary>
        /// 从登录凭证中获取到用户信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        T GetSingle(HttpContext? httpContext = null);
        /// <summary>
        /// 从缓存中心获取到用户信息，如果有设置缓存的话，
        /// 如果没有，同GetSingle
        /// </summary>
        /// <returns></returns>
        T Get();
        /// <summary>
        /// 设置登录凭证到token
        /// </summary>
        /// <param name="tadmin"></param>
        void Set(T tadmin);

        bool IsLogin();

        void LoginOut();
    }

    /// <summary>
    /// 是否把该字段记录到jwttoken中
    /// </summary>
    public class JwtKeyAttribute : Attribute
    {
        public static bool ContainMyAttribute(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(JwtKeyAttribute)).FirstOrDefault();
            if (attr==null) return false;
            else return true;
        }
    }

    public interface IToken
    { 
        string GUID { get; set; }
        string Token { get; set; }
    }
}