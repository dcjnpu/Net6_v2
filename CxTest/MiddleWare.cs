using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Transactions;

namespace CxTest
{
    /// <summary>
    /// 中间件
    /// </summary>
    public class TestMiddlware : IMiddleware
    {
        #region 据说可以不继承 IMiddleware 接口
        private readonly RequestDelegate Next;

        public TestMiddlware(RequestDelegate next)
        {
            Next=next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //string? jwt = context.Request.Headers["Authorization"];
            ////Dynamic.Json 使用
            ////var djson = Dynamic.Json.DJson.Parse("");
            ////
            //if (string.IsNullOrEmpty(jwt))
            //{

            //    context.Response.StatusCode =401;
            //}
            //else
            //{
            //    Console.WriteLine("md1 start");
            //    await Next.Invoke(context);
            //    Console.WriteLine("md2 start");
            //}

            Console.WriteLine("md1.1 start");
            await Next.Invoke(context);
            Console.WriteLine("md1.2 start");
        }
        #endregion

        /// <summary>
        /// 用接口的方式，
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine("md1.21 start");
            await next.Invoke(context);
            Console.WriteLine("md1.22 start");
        }
    }

    public class TestMiddlware2
    {
        /// <summary>
        /// 直接委托的方式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static async Task Test1(HttpContext context, Func<Task> next)
        {
            Console.WriteLine("md1.21 start");
            await next.Invoke();
            Console.WriteLine("md1.22 start");
        }
    }

    public static class MiddleWareExtensions
    {
        /// <summary>
        /// 添加中间件测试
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseTestMiddleware(this IApplicationBuilder applicationBuilder)
        {
            //1，2中方式
            applicationBuilder.UseMiddleware<TestMiddlware>();
            //独立接入
            applicationBuilder.Use(async (context, next) =>
            {
                Console.WriteLine("md2 start");
                await next.Invoke();
                Console.WriteLine("md2 end");
            });
            //方式3 直接委托的方式
            applicationBuilder.Use(TestMiddlware2.Test1);
        }
    }
}