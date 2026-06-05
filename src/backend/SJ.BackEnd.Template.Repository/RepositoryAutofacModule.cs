using Autofac;

namespace SJ.BackEnd.Template.Repository;

/// <summary>
/// Repository 层的依赖注入模块
/// <para>本模块由 WebAPI 运行时自动扫描发现并注册，无需在宿主项目中手动配置。</para>
/// </summary>
public class RepositoryAutofacModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册泛型仓储
        builder.RegisterGeneric(typeof(BaseRepository<>))
            .As(typeof(IBaseRepository<>))
            .InstancePerDependency();

        // 批量注册本程序集中所有仓储实现
        var assembly = typeof(RepositoryAutofacModule).Assembly;
        builder.RegisterAssemblyTypes(assembly)
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerDependency();

        // 注册工作单元
        builder.RegisterType<UnitOfWorkManage>()
            .As<IUnitOfWorkManage>()
            .InstancePerLifetimeScope()
            .PropertiesAutowired();
    }
}
