#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysRoleController.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysRoleController
* 创 建 者：何应芝
* 创建时间：2026/6/8 22:00:00
* ==============================================================================*/
#endregion

using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysRole;

namespace SJ.BackEnd.Template.WebAPI.Controllers;

[ApiController]
[Route("role")]
public class SysRoleController(ISysRoleService sysRoleService) : ControllerBase
{
    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PageModel<SysRole>>> GetList([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var result = await sysRoleService.GetPagedList(pageIndex, pageSize, keyword);
        return ApiResponse<PageModel<SysRole>>.Success("查询成功", result);
    }

    /// <summary>
    /// 根据 ID 获取角色详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysRole>> GetById(long id)
    {
        var role = await sysRoleService.GetById(id);
        if (role == null)
            return ApiResponse<SysRole>.Fail("角色不存在");
        return ApiResponse<SysRole>.Success("获取成功", role);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<SysRole>> Create([FromBody] CreateRoleRequest request)
    {
        var result = await sysRoleService.Create(request);
        if (result == null)
            return ApiResponse<SysRole>.Fail("角色名称或编码已存在");
        return ApiResponse<SysRole>.Success("创建成功", result);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateRoleRequest request)
    {
        var result = await sysRoleService.Update(id, request);
        if (!result)
            return ApiResponse<bool>.Fail("角色不存在或名称已存在");
        return ApiResponse<bool>.Success("更新成功", true);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var role = await sysRoleService.GetById(id);
        if (role == null)
            return ApiResponse<bool>.Fail("角色不存在");

        if (role.IsSystem)
            return ApiResponse<bool>.Fail("系统内置角色不能删除");

        var result = await sysRoleService.Delete(id);
        return ApiResponse<bool>.Success("删除成功", result);
    }
}
