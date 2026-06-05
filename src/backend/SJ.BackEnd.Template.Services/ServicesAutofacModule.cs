using Autofac;
using Autofac.Extras.DynamicProxy;

namespace SJ.BackEnd.Template.Services;

/// <summary>
/// Services 层的依赖注入模块
/// <para>本模块由 WebAPI 运行时自动扫描发现并注册，无需在宿主项目中手动配置。</para>
/// </summary>
public class ServicesAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册泛型服务
        builder.RegisterGeneric(typeof(BaseServices<>))
            .As(typeof(IBaseServices<>))
            .InstancePerDependency();

        // 批量注册本程序集中所有服务实现
        var assembly = typeof(ServicesAutofacModule).Assembly;
        builder.RegisterAssemblyTypes(assembly)
            .AsImplementedInterfaces()
            .InstancePerDependency()
            .PropertiesAutowired()
            .EnableInterfaceInterceptors();
    }
}
