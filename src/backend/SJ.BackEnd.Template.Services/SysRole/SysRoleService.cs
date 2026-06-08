#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysRoleService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysRoleService
* 创 建 者：何应芝
* 创建时间：2026/6/8 22:00:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Common.Extensions;
using SJ.BackEnd.Template.Model.Dtos.SysRole;

namespace SJ.BackEnd.Template.Services;

public class SysRoleService(IBaseRepository<SysRole> repository) : BaseServices<SysRole>(repository), ISysRoleService
{
    public async Task<PageModel<SysRole>> GetPagedList(int pageIndex, int pageSize, string? keyword)
    {
        Expression<Func<SysRole, bool>> whereExpression = _ => true;
        whereExpression = whereExpression.WhereIF(!string.IsNullOrWhiteSpace(keyword),
            u => u.Name.Contains(keyword) || u.Code.Contains(keyword));

        string orderByFields = "SortOrder asc, Id desc";
        return await base.GetPagedListByExpression(whereExpression, pageIndex, pageSize, orderByFields);
    }

    public async Task<SysRole?> Create(CreateRoleRequest request)
    {
        // 名称唯一性校验
        var exists = await base.QueryByExpression(u => u.Name == request.Name);
        if (exists.Any())
            return null;

        var role = new SysRole
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description ?? string.Empty,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var newId = await base.Insert(role);
        return await base.GetById(newId);
    }

    public async Task<bool> Update(long id, UpdateRoleRequest request)
    {
        // 名称唯一性校验（排除自身）
        var exists = await base.QueryByExpression(u => u.Name == request.Name && u.Id != id);
        if (exists.Any())
            return false;

        var role = await base.GetById(id);
        if (role == null)
            return false;

        role.Name = request.Name;
        role.Code = request.Code;
        role.Description = request.Description ?? string.Empty;
        role.SortOrder = request.SortOrder;
        role.UpdatedAt = DateTime.Now;

        return await base.Update(role);
    }

    public new async Task<bool> Delete(long id)
    {
        return await base.DeleteById(id);
    }
}
