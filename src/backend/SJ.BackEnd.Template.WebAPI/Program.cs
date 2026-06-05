using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using SJ.BackEnd.Template.Common;
using SJ.BackEnd.Template.Common.DB;
using SJ.BackEnd.Template.Extensions;
using SJ.BackEnd.Template.Extensions.ServiceExtensions;
using SJ.BackEnd.Template.WebAPI;

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
});

builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());


Console.WriteLine(AppSettings.App("Serilog", "MinimumLevel", "Default"));

var app = builder.Build();

// 默认文件中间件
DefaultFilesOptions defaultFilesOptions = new();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
// 静态文件中间件
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseSerilogRequestLogging();
// app.UseStatusCodePages();

app.UseAuthorization();

app.MapControllers();

app.Run();
