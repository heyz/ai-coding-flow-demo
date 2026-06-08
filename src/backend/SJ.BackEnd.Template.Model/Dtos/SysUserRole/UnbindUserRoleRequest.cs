#region  <<版本注释>>
/* ==============================================================================
// <copyright file="UnbindUserRoleRequest.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：UnbindUserRoleRequest
* 创 建 者：何应芝
* 创建时间：2026/6/8 23:00:00
* ==============================================================================*/
#endregion

using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysUserRole;

/// <summary>
/// 解绑用户角色请求 DTO
/// </summary>
public class UnbindUserRoleRequest
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "用户ID必须大于0")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "角色ID必须大于0")]
    public long RoleId { get; set; }
}
