using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Cx.Jwt
{
    /// <summary>
    /// 使用测试
    /// </summary>
    public class TestUse : LoginToken_Jwt<TAdmin>, IAdmin<TAdmin>
    {
        public TestUse(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
            //this.admin=admin;
        }
    }

    public static class CxJwtExtensions
    {
        public static IServiceCollection AddMyJwt(this IServiceCollection services)
        {
            return services.AddSingleton<TestUse>();//另一种用法
        }

        public static IServiceCollection AddMyJwt2(this IServiceCollection services)
        {
            return services.AddSingleton<IAdmin<TAdmin>, LoginToken_Jwt<TAdmin>>();
        }
    }

    public interface ITestUse
    {

    }

    public class TAdmin : IToken
    {
        private string _token = string.Empty;
        private string _guid = string.Empty;
        public string GUID { get => _guid; set => _guid=value; }
        public string Token { get => _token; set => _token=value; }
    }
}