using System.Reflection;
using Autofac;
using AutofacModule = Autofac.Module;
using Microsoft.AspNetCore.Mvc;

namespace SJ.BackEnd.Template.WebAPI;

/// <summary>
/// WebAPI 组合根模块：只注册控制器 + 自动发现各实现层的 Module
/// <para>
/// 各实现层（Services、Repository）自带 Autofac Module，
/// 本模块会扫描 bin 目录自动加载它们，无需手动引用实现层。
/// </para>
/// </summary>
public class WebAPIAutofacModule : AutofacModule
{
    protected override void Load(ContainerBuilder builder)
    {
        // ==================== 控制器注册 ====================
        var controllerBaseType = typeof(ControllerBase);
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();

        // ==================== 自动发现各实现层的 Module ====================
        var baseDirectory = AppContext.BaseDirectory;
        var prefix = "SJ.BackEnd.Template.";

        foreach (var dllFile in Directory.GetFiles(baseDirectory, $"{prefix}*.dll"))
        {
            var assemblyName = Path.GetFileNameWithoutExtension(dllFile);
            if (assemblyName == null) continue;

            // 跳过接口层和基础设施层（它们没有实现类需要注册）
            if (assemblyName == $"{prefix}IServices"
                || assemblyName == $"{prefix}IRepository"
                || assemblyName == $"{prefix}Extensions"
                || assemblyName == $"{prefix}Common"
                || assemblyName == $"{prefix}Model"
                || assemblyName == $"{prefix}WebAPI")
                continue;

            var assembly = Assembly.LoadFrom(dllFile);

            var moduleTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AutofacModule)) && !t.IsAbstract);

            foreach (var moduleType in moduleTypes)
            {
                builder.RegisterModule((AutofacModule)Activator.CreateInstance(moduleType)!);
            }
        }
    }
}
