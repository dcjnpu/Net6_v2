using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cx.Data
{
    /// <summary>
    /// HttpContext的扩展
    /// </summary>
    public static class CxHttpContextExtensions
    {
        //public static IServiceCollection serviceCollection;

        //public static Microsoft.AspNetCore.Http.HttpContext Current
        //{
        //    get
        //    {
        //        object factory = serviceCollection.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
        //        //var ifactory = serviceCollection.BuildServiceProvider().GetService(typeof(IHttpContextFactory)) as IHttpContextFactory;
        //        //ifactory.Create
        //        HttpContext context = ((HttpContextAccessor)factory).HttpContext;
        //        return context;
        //    }
        //}

        //public static IT GetService<IT>()
        //{
        //    object factory = serviceCollection.BuildServiceProvider().GetService(typeof(IT));
        //    IT context = (IT)factory;
        //    return context;
        //}

        /// <summary>
        /// 习惯用法，取到HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                var context = CxCurrentService.GetService<IHttpContextAccessor>();
                return context.HttpContext;
            }
        }

        /// <summary>
        /// 返回IP
        /// </summary>
        /// <param name="_Current"></param>
        /// <returns></returns>
        public static string IP(this HttpContext _Current)
        {
            return _Current.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// 返回IP
        /// </summary>
        /// <returns></returns>
        public static string IP()
        {
            return Current.Connection.RemoteIpAddress.ToString();
        }
    }

    /// <summary>
    /// 手动后去实例，
    /// </summary>
    public static class CxCurrentService
    {
        /// <summary>
        /// 实例容器
        /// </summary>
        public static IServiceProvider? Instance { get; set; }

        /// <summary>
        /// 返回需要的实例 只针对Singleton\Transient
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <returns></returns>
        public static IT GetService<IT>()
        {
            var valueService = Instance!.GetService<IT>()!;
            return valueService;
        }

        /// <summary>
        /// 获取Scoped的实例
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static IT GetScopeService<IT>(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.RequestServices.GetService<IT>()!;
        }

        /// <summary>
        ///  获取Scoped的实例
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <returns></returns>
        public static IT GetScopeService<IT>()
        {
            IHttpContextAccessor httpContextAccessor = GetService<IHttpContextAccessor>();
            return httpContextAccessor.HttpContext.RequestServices.GetService<IT>()!;
        }

    }
}