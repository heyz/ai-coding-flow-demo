#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysPermissionController.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysPermissionController
* 创 建 者：何应芝
* 创建时间：2026/6/9 00:00:00
* ==============================================================================*/
#endregion

using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysPermission;

namespace SJ.BackEnd.Template.WebAPI.Controllers;

[ApiController]
[Route("permission")]
public class SysPermissionController(ISysPermissionService sysPermissionService) : ControllerBase
{
    [HttpGet("tree")]
    public async Task<ApiResponse<List<SysPermission>>> GetTree()
    {
        var list = await sysPermissionService.GetTree();
        return ApiResponse<List<SysPermission>>.Success("查询成功", list);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<SysPermission>> GetById(long id)
    {
        var permission = await sysPermissionService.GetById(id);
        if (permission == null)
            return ApiResponse<SysPermission>.Fail("权限不存在");
        return ApiResponse<SysPermission>.Success("获取成功", permission);
    }

    [HttpPost]
    public async Task<ApiResponse<SysPermission>> Create([FromBody] CreatePermissionRequest request)
    {
        var result = await sysPermissionService.Create(request);
        if (result == null)
            return ApiResponse<SysPermission>.Fail("权限编码已存在");
        return ApiResponse<SysPermission>.Success("创建成功", result);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdatePermissionRequest request)
    {
        var result = await sysPermissionService.Update(id, request);
        if (!result)
            return ApiResponse<bool>.Fail("权限不存在或编码已存在");
        return ApiResponse<bool>.Success("更新成功", true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var permission = await sysPermissionService.GetById(id);
        if (permission == null)
            return ApiResponse<bool>.Fail("权限不存在");

        var result = await sysPermissionService.Delete(id);
        if (!result)
            return ApiResponse<bool>.Fail("该权限存在子节点，无法删除");
        return ApiResponse<bool>.Success("删除成功", true);
    }
}
