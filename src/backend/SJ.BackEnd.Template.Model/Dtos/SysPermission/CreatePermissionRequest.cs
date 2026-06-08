#region  <<版本注释>>
/* ==============================================================================
// <copyright file="CreatePermissionRequest.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：CreatePermissionRequest
* 创 建 者：何应芝
* 创建时间：2026/6/9 00:00:00
* ==============================================================================*/
#endregion

using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysPermission;

public class CreatePermissionRequest
{
    [Required(ErrorMessage = "权限名称不能为空")]
    [StringLength(50, ErrorMessage = "权限名称长度不能超过{1}个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "权限编码不能为空")]
    [StringLength(100, ErrorMessage = "权限编码长度不能超过{1}个字符")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "权限类型不能为空")]
    [RegularExpression("^(menu|button|api)$", ErrorMessage = "权限类型必须是 menu/button/api")]
    public string Type { get; set; } = "menu";

    public long ParentId { get; set; } = 0;

    [StringLength(200)]
    public string? Path { get; set; }

    [StringLength(50)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; } = 0;
}
