using BaseBLL;
using Cx.Redis;
using Cx.SqlSugarV2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//ISqlSugererver baseService = new BaseService("DbConnection:ConnectionString", GlobalSetting.SqlSugarDbType);
//builder.Services.AddSingleton(baseService);
builder.Services.AddSingleton<IPubSugar, PubSugar>();
builder.Services.AddSingleton<IProjectCache, ProjectCache>();

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
            bool flag = context.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues head_token);
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
            return Task.CompletedTask;
        },
        //������ѯ��ʱ���ڽ�ѯ�ʷ��ͻص��÷�֮ǰ����   ���Բ�����
        OnChallenge= context => { return Task.CompletedTask; },
        //��Ϊ��ֹ��������Ȩʧ�ܵ���    ���Բ�����
        OnForbidden= context => { return Task.CompletedTask; },
        //��Token��֤ͨ�������
        OnTokenValidated= context => { return Task.CompletedTask; }
    };
});

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
/// <summary>
/// ������֤
/// </summary>
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();