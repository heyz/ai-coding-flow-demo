#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ISysPermissionService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ISysPermissionService
* 创 建 者：何应芝
* 创建时间：2026/6/9 00:00:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysPermission;

namespace SJ.BackEnd.Template.IServices;

public interface ISysPermissionService : IBaseServices<SysPermission>
{
    Task<SysPermission?> Create(CreatePermissionRequest request);
    Task<bool> Update(long id, UpdatePermissionRequest request);
    new Task<bool> Delete(long id);
    Task<List<SysPermission>> GetTree();
}
