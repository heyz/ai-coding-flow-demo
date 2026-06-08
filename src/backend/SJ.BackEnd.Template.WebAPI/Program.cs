using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using SJ.BackEnd.Template.Common;
using SJ.BackEnd.Template.Common.DB;
using SJ.BackEnd.Template.Extensions;
using SJ.BackEnd.Template.Extensions.ServiceExtensions;
using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.WebAPI;
using SJ.BackEnd.Template.WebAPI.Validators;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

var serilogConfig = builder.Configuration.GetSection("Serilog").Get<SerilogConfig>() 
    ?? new SerilogConfig();

Log.Logger = new LoggerConfiguration()
    .SetupSerilogSinks(serilogConfig)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host
    .UseSerilog()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<WebAPIAutofacModule>();
    });

builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSqlsugarSetup(builder.Configuration.GetSection("DBS").Get<List<ConfigDbItem>>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddCors(opts => { 
    opts.AddPolicy("AllowAll", policy => { 
        policy.SetIsOriginAllowed(host=>true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers(o => {
    o.Filters.Add(typeof(GlobalExceptionsFilter));
    o.Filters.Add(typeof(ValidationFilter));
}).ConfigureApiBehaviorOptions(o => {
    o.SuppressModelStateInvalidFilter = true;
});

builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());


Console.WriteLine(AppSettings.App("Serilog", "MinimumLevel", "Default"));

var app = builder.Build();

// Ĭ���ļ��м��
DefaultFilesOptions defaultFilesOptions = new();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
// ��̬�ļ��м��
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseSerilogRequestLogging();
// app.UseStatusCodePages();

app.UseAuthorization();

app.MapControllers();

// CodeFirst: 自动创建或更新数据表
using (var scope = app.Services.CreateScope())
{
    var sqlSugarScope = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>() as SqlSugarScope;
    var db = sqlSugarScope!.GetConnectionScope("2");
    db.CodeFirst.InitTables<SysRole, SysUserRole, SysPermission>();
}

app.Run();
