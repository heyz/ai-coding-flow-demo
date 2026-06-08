#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysPermissionService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysPermissionService
* 创 建 者：何应芝
* 创建时间：2026/6/9 00:00:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Common.Extensions;
using SJ.BackEnd.Template.Model.Dtos.SysPermission;

namespace SJ.BackEnd.Template.Services;

public class SysPermissionService(IBaseRepository<SysPermission> repository)
    : BaseServices<SysPermission>(repository), ISysPermissionService
{
    public async Task<SysPermission?> Create(CreatePermissionRequest request)
    {
        var exists = await base.QueryByExpression(u => u.Code == request.Code);
        if (exists.Any())
            return null;

        var permission = new SysPermission
        {
            Name = request.Name,
            Code = request.Code,
            Type = request.Type,
            ParentId = request.ParentId,
            Path = request.Path,
            Icon = request.Icon,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var newId = await base.Insert(permission);
        return await base.GetById(newId);
    }

    public async Task<bool> Update(long id, UpdatePermissionRequest request)
    {
        var exists = await base.QueryByExpression(u => u.Code == request.Code && u.Id != id);
        if (exists.Any())
            return false;

        var permission = await base.GetById(id);
        if (permission == null)
            return false;

        permission.Name = request.Name;
        permission.Code = request.Code;
        permission.Type = request.Type;
        permission.ParentId = request.ParentId;
        permission.Path = request.Path;
        permission.Icon = request.Icon;
        permission.SortOrder = request.SortOrder;
        permission.UpdatedAt = DateTime.Now;

        return await base.Update(permission);
    }

    public new async Task<bool> Delete(long id)
    {
        // 检查是否有子节点
        var children = await base.QueryByExpression(u => u.ParentId == id);
        if (children.Any())
            return false;

        return await base.DeleteById(id);
    }

    public async Task<List<SysPermission>> GetTree()
    {
        return await base.QueryByExpression(_ => true);
    }
}
