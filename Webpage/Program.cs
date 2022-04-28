using BaseBLL;
using Cx.Data;
using Cx.Redis;
using Cx.SqlSugarV2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    //不转换大小写
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//ISqlSugererver baseService = new BaseService("DbConnection:ConnectionString", GlobalSetting.SqlSugarDbType);
//builder.Services.AddSingleton(baseService);


//builder.Services.AddScoped<IAdmin, Admin_Cookie>();
builder.Services.AddSession()
    .AddCxData()
    .AddCxRedis()
    .AddCxSugar()
    .AddSingleton<Admin_MyJwt>()
    ;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
    };

    options.Events = new JwtBearerEvents()
    {
        //验证是否传入token..主要为了从非Authorization 中获取到token
        OnMessageReceived = context =>
        {
            var key_token = builder.Configuration["JWT:CookieKey"];
            bool flag = context.Request.Cookies.TryGetValue(key_token, out string? head_token);
            if (flag)
            {
                context.Token = head_token; return Task.CompletedTask;
            }
            context.Token = "";
            return Task.CompletedTask;
        },
        //认证失败后调用
        OnAuthenticationFailed = context =>
        {
            context.Response.Redirect("/home/login");
            return Task.CompletedTask;
        },
        ////发生质询的时，在将询问发送回调用方之前调用   可以不处理
        //OnChallenge= context => { return Task.CompletedTask; },
        ////因为禁止而导致授权失败调用    可以不处理
        OnForbidden = context =>
         {
             context.Response.Redirect("/home/login");
             return Task.CompletedTask;
         },
        ////在Token验证通过后调用
        //OnTokenValidated= context => { return Task.CompletedTask; }
    };
});

#region 图片上传

builder.Services.AddHttpClient("upload_net6", config => //这里指定的 name=upload_net6 ，可以方便我们后期复用该实例
{
    config.BaseAddress = new Uri("http://img.jhrqsh.com/upload");
    //config.DefaultRequestHeaders.Add("ContentType", "header_1");
});
builder.Services.AddSingleton<Cx.Data.FileUpload>();

#endregion 图片上传

//builder.Services.AddAuthentication(option =>
//{
//    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; //默认身份验证方案
//    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
//{
//    option.LoginPath = "/Home/Login";//如果没有找到用户信息---身份验证失败--授权也失败了---就跳转到指定的Action
//    option.AccessDeniedPath = "/Home/NoAuthority";
//});

//AddAssembly(builder.Services, "BLL");

//var  serviceProvider = builder.Services.BuildServiceProvider().GetRequiredService<ISqlSugarHelper>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();
/// <summary>
/// 启用认证
/// </summary>
app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.Response.StatusCode == 401) { context.Response.Clear(); context.Response.Redirect("/home/login"); return; }
    //Console.WriteLine("md2 start");
    await next();
    //
    //Console.WriteLine("md2 end");
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#region 自定义部分
var baseService = new PubSugar().Default;
AdminStartup.InitBaseTable(baseService);//初始化 主表
AdminStartup.InitTableData(baseService);//填入表数据
IPubSugar pubSugar = app.Services.GetService<IPubSugar>()!;
AdminStartup.InitOtherTable(pubSugar);
Cx.Data.CxCurrentService.Instance = app.Services;

#endregion 自定义部分

app.Run();

/// <summary>
/// Ioc
/// </summary>
/// <param name="services">services</param>
/// <param name="assemblyName">assemblyName</param>
void AddAssembly(IServiceCollection services, string assemblyName)
{
    if (!String.IsNullOrEmpty(assemblyName))
    {
        Assembly assembly = Assembly.Load(assemblyName);
        List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();
        //foreach (var item in ts.Where(s => !s.IsInterface&& typeof(I_TB_Split_Base).IsAssignableFrom(s)))
        foreach (var item in ts.Where(s => !s.IsInterface))
        {
            var interfaceType = item.GetInterfaces();
            if (interfaceType.Length == 1)
            {
                services.AddTransient(interfaceType[0], item);
            }
            if (interfaceType.Length > 1)
            {
                services.AddTransient(interfaceType[1], item);
            }
        }
    }
}