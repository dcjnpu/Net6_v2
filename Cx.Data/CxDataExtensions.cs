using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Cx.Data
{
    /// <summary>
    /// 注入扩展
    /// </summary>
    public static class CxDataExtensions
    {
        /// <summary>
        /// CxData的DI
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddCxData(this IServiceCollection services)
        {
            return services.AddTransient<IImageCaptcha, ImageCaptcha>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<ICacheService, MemoryCacheService>()
                ;
        }
    }
    //public class SampleDomainService : IDomainService
    //{
    //    private IUnitOfWork _unitOfWork;

    //    public SampleDomainService(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _unitOfWork = httpContextAccessor.HttpContext.RequestServices.GetService<IUnitOfWork>();
    //    }
    //}
}
