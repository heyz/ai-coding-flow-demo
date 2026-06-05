#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SqlsugarSetup.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SqlsugarSetup
* 创 建 者：何应芝
* 创建时间：2026/5/25 16:55:56
* ==============================================================================*/
#endregion

using Microsoft.Extensions.DependencyInjection;
using SJ.BackEnd.Template.Common.DB;
using SqlSugar;

namespace SJ.BackEnd.Template.Extensions;

/// <summary>
/// SqlSugar 启动服务
/// </summary>
public static class SqlsugarSetup
{
    public static void AddSqlsugarSetup(this IServiceCollection services, List<ConfigDbItem>? items)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(items);
        
        var configs = new List<ConnectionConfig>(items.Count);

        foreach (var item in items)
        {
            var config = new ConnectionConfig
            {
                ConfigId = item.ConnId,
                ConnectionString = item.Connection,
                DbType = (DbType)item.DbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
               
            };

            configs.Add(config);
        }

        services.AddSingleton<ISqlSugarClient>(o =>
        {
            return new SqlSugarScope(configs, db =>
            {
                db.Aop.OnLogExecuting = (sql, parameters) => // SQL执行前
                {
                   Console.WriteLine(UtilMethods.GetNativeSql(sql, parameters));
                };

                db.Aop.OnError = (exp) => //SQL报错
                {
                    Console.WriteLine(UtilMethods.GetSqlString(DbType.MySql, exp.Sql, exp.Parametres as SugarParameter[]));
                };
                
            });
        });

        // services.AddTransient<SqlSugarScope>(s => s.GetRequiredService<ISqlSugarClient>() as SqlSugarScope);
    }

}
