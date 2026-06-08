#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysUserRole.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysUserRole
* 创 建 者：何应芝
* 创建时间：2026/6/8 23:00:00
* ==============================================================================*/
#endregion

using SqlSugar;

namespace SJ.BackEnd.Template.Model;

[SugarTable("sys_user_role")]
[Tenant("2")]
public class SysUserRole
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long RoleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
