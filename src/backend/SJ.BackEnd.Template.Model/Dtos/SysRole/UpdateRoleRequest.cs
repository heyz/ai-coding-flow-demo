#region  <<版本注释>>
/* ==============================================================================
// <copyright file="UpdateRoleRequest.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：UpdateRoleRequest
* 创 建 者：何应芝
* 创建时间：2026/6/8 22:00:00
* ==============================================================================*/
#endregion

using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysRole;

/// <summary>
/// 修改角色请求 DTO
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, ErrorMessage = "角色名称长度不能超过{1}个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(50, ErrorMessage = "角色编码长度不能超过{1}个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(200, ErrorMessage = "描述长度不能超过{1}个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
}
