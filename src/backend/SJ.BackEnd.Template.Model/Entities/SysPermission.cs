#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysPermission.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysPermission
* 创 建 者：何应芝
* 创建时间：2026/6/9 00:00:00
* ==============================================================================*/
#endregion

using SqlSugar;

namespace SJ.BackEnd.Template.Model;

[SugarTable("sys_permission")]
[Tenant("2")]
public class SysPermission
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(Length = 50)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>menu / button / api</summary>
    [SugarColumn(Length = 20)]
    public string Type { get; set; } = "menu";

    public long ParentId { get; set; } = 0;

    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
