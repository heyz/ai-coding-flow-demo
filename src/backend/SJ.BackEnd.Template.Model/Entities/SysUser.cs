#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysUser.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysUser
* 创 建 者：何应芝
* 创建时间：2026/6/5 15:15:00
* ==============================================================================*/
#endregion

using SqlSugar;

namespace SJ.BackEnd.Template.Model;

[SugarTable("sys_user")]
[Tenant("2")]
public class SysUser
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// 性别 (1-男, 2-女, 0-未知)
    /// </summary>
    public int Gender { get; set; } = 0;

    /// <summary>
    /// 出生年月
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}
