#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ISysRoleService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ISysRoleService
* 创 建 者：何应芝
* 创建时间：2026/6/8 22:00:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysRole;

namespace SJ.BackEnd.Template.IServices;

public interface ISysRoleService : IBaseServices<SysRole>
{
    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    Task<PageModel<SysRole>> GetPagedList(int pageIndex, int pageSize, string? keyword);

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <returns>新角色对象，名称/编码重复时返回 null</returns>
    Task<SysRole?> Create(CreateRoleRequest request);

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <returns>是否更新成功</returns>
    Task<bool> Update(long id, UpdateRoleRequest request);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <returns>是否删除成功</returns>
    new Task<bool> Delete(long id);
}
