#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysUserRoleController.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysUserRoleController
* 创 建 者：何应芝
* 创建时间：2026/6/8 23:00:00
* ==============================================================================*/
#endregion

using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysUserRole;

namespace SJ.BackEnd.Template.WebAPI.Controllers;

[ApiController]
[Route("user-role")]
public class SysUserRoleController(ISysUserRoleService sysUserRoleService, ISysUserService sysUserService, ISysRoleService sysRoleService) : ControllerBase
{
    /// <summary>
    /// 绑定用户到角色
    /// </summary>
    [HttpPost("bind")]
    public async Task<ApiResponse<bool>> Bind([FromBody] BindUserRoleRequest request)
    {
        var user = await sysUserService.GetById(request.UserId);
        if (user == null)
            return ApiResponse<bool>.Fail("用户不存在");

        var role = await sysRoleService.GetById(request.RoleId);
        if (role == null)
            return ApiResponse<bool>.Fail("角色不存在");

        var result = await sysUserRoleService.Bind(request.UserId, request.RoleId);
        if (!result)
            return ApiResponse<bool>.Fail("该用户已绑定此角色");
        return ApiResponse<bool>.Success("绑定成功", true);
    }

    /// <summary>
    /// 解绑用户角色
    /// </summary>
    [HttpPost("unbind")]
    public async Task<ApiResponse<bool>> Unbind([FromBody] UnbindUserRoleRequest request)
    {
        var result = await sysUserRoleService.Unbind(request.UserId, request.RoleId);
        if (!result)
            return ApiResponse<bool>.Fail("解绑失败，绑定关系不存在");
        return ApiResponse<bool>.Success("解绑成功", true);
    }

    /// <summary>
    /// 查询指定用户的所有角色
    /// </summary>
    [HttpGet("user/{userId}/roles")]
    public async Task<ApiResponse<List<SysRole>>> GetRolesByUserId(long userId)
    {
        var user = await sysUserService.GetById(userId);
        if (user == null)
            return ApiResponse<List<SysRole>>.Fail("用户不存在");

        var roles = await sysUserRoleService.GetRolesByUserId(userId);
        return ApiResponse<List<SysRole>>.Success("查询成功", roles);
    }

    /// <summary>
    /// 查询指定角色下的所有用户
    /// </summary>
    [HttpGet("role/{roleId}/users")]
    public async Task<ApiResponse<List<SysUser>>> GetUsersByRoleId(long roleId)
    {
        var role = await sysRoleService.GetById(roleId);
        if (role == null)
            return ApiResponse<List<SysUser>>.Fail("角色不存在");

        var users = await sysUserRoleService.GetUsersByRoleId(roleId);
        return ApiResponse<List<SysUser>>.Success("查询成功", users);
    }
}
