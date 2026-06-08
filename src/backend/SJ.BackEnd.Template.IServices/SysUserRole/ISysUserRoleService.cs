#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ISysUserRoleService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ISysUserRoleService
* 创 建 者：何应芝
* 创建时间：2026/6/8 23:00:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysUserRole;

namespace SJ.BackEnd.Template.IServices;

public interface ISysUserRoleService : IBaseServices<SysUserRole>
{
    /// <summary>
    /// 绑定用户到角色
    /// </summary>
    Task<bool> Bind(long userId, long roleId);

    /// <summary>
    /// 解绑用户角色
    /// </summary>
    Task<bool> Unbind(long userId, long roleId);

    /// <summary>
    /// 查询指定用户的所有角色
    /// </summary>
    Task<List<SysRole>> GetRolesByUserId(long userId);

    /// <summary>
    /// 查询指定角色下的所有用户
    /// </summary>
    Task<List<SysUser>> GetUsersByRoleId(long roleId);
}
