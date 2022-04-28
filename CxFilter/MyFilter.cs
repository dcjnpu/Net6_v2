global using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CxFilter;

/*
 Filter:program里配置
 */
public static class Exsensions
{
    /// <summary>
    /// 注入filter写法
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection AddFilter(this IServiceCollection service)
    {
        service.Configure<MvcOptions>(opt => {
            opt.Filters.Add<MyActionFilterAsync>();
            opt.Filters.Add<MyExceptionFilterAsync>();
        });
        return service;
    }
}

public class MyActionFilterAsync : IAsyncActionFilter
{
    Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //typeof(a).GetCustomAttributes
        next.Invoke();
        throw new NotImplementedException();
    }

    //public override void on
}




public class MyExceptionFilterAsync : IAsyncExceptionFilter
{
    Task IAsyncExceptionFilter.OnExceptionAsync(ExceptionContext context)
    {
        //context.Exception//异常对象
        //context.ExceptionHandled = true;//是否已处理异常，true后面其他异常就不再铺货
        //context.Result = null;//返回客户端的值

        throw new NotImplementedException();
    }
}