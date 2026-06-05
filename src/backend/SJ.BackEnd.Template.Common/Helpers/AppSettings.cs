#region  <<版本注释>>
/* ==============================================================================
// <copyright file="AppSetting.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：AppSetting
* 创 建 者：何应芝
* 创建时间：2026/5/29 15:42:39
* ==============================================================================*/
#endregion

using Microsoft.Extensions.Configuration;

namespace SJ.BackEnd.Template.Common;

/// <summary>
/// appsettings.json操作类
/// </summary>
public class AppSettings
{
    public static IConfiguration Configuration { get; set; }
  
    public AppSettings(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// 封装要操作的字符
    /// </summary>
    /// <param name="sections">节点配置</param>
    /// <returns></returns>
    public static string App(params string[] sections)
    {
        try
        {
            if (sections.Length != 0)
            {
                return Configuration[string.Join(":", sections)] ?? string.Empty;
            }
        }
        catch (Exception)
        {
        }

        return string.Empty;
    }

    /// <summary>
    /// 递归获取配置信息数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sections"></param>
    /// <returns></returns>
    public static List<T> App<T>(params string[] sections)
    {
        List<T> list = [];
        // 引用 Microsoft.Extensions.Configuration.Binder 包
        Configuration.Bind(string.Join(":", sections), list);
        return list;
    }


    /// <summary>
    /// 根据路径  configuration["App:Name"];
    /// </summary>
    /// <param name="sectionsPath"></param>
    /// <returns></returns>
    public static string GetValue(string sectionsPath)
    {
        try
        {
            return Configuration[sectionsPath] ?? string.Empty;
        }
        catch (Exception)
        {
        }

        return string.Empty;
    }
}