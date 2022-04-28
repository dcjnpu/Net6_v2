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
    //��ת����Сд
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
        //��֤�Ƿ���token..��ҪΪ�˴ӷ�Authorization �л�ȡ��token
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
        //��֤ʧ�ܺ����
        OnAuthenticationFailed = context =>
        {
            context.Response.Redirect("/home/login");
            return Task.CompletedTask;
        },
        ////������ѯ��ʱ���ڽ�ѯ�ʷ��ͻص��÷�֮ǰ����   ���Բ�����
        //OnChallenge= context => { return Task.CompletedTask; },
        ////��Ϊ��ֹ��������Ȩʧ�ܵ���    ���Բ�����
        OnForbidden = context =>
         {
             context.Response.Redirect("/home/login");
             return Task.CompletedTask;
         },
        ////��Token��֤ͨ�������
        //OnTokenValidated= context => { return Task.CompletedTask; }
    };
});

#region ͼƬ�ϴ�

builder.Services.AddHttpClient("upload_net6", config => //����ָ���� name=upload_net6 �����Է������Ǻ��ڸ��ø�ʵ��
{
    config.BaseAddress = new Uri("http://img.jhrqsh.com/upload");
    //config.DefaultRequestHeaders.Add("ContentType", "header_1");
});
builder.Services.AddSingleton<Cx.Data.FileUpload>();

#endregion ͼƬ�ϴ�

//builder.Services.AddAuthentication(option =>
//{
//    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; //Ĭ�������֤����
//    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
//{
//    option.LoginPath = "/Home/Login";//���û���ҵ��û���Ϣ---�����֤ʧ��--��ȨҲʧ����---����ת��ָ����Action
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
/// ������֤
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

#region �Զ��岿��
var baseService = new PubSugar().Default;
AdminStartup.InitBaseTable(baseService);//��ʼ�� ����
AdminStartup.InitTableData(baseService);//���������
IPubSugar pubSugar = app.Services.GetService<IPubSugar>()!;
AdminStartup.InitOtherTable(pubSugar);
Cx.Data.CxCurrentService.Instance = app.Services;

#endregion �Զ��岿��

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