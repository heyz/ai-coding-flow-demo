#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ISysUserService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ISysUserService
* 创 建 者：何应芝
* 创建时间：2026/6/5 16:30:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysUser;

namespace SJ.BackEnd.Template.IServices;

public interface ISysUserService : IBaseServices<SysUser>
{
    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="keyword">搜索关键词（真实姓名/昵称）</param>
    /// <returns>分页结果</returns>
    Task<PageModel<SysUser>> GetPagedList(int pageIndex, int pageSize, string? keyword);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request">创建用户请求</param>
    /// <returns>新用户信息，昵称重复时返回 null</returns>
    Task<CreateUserResponse?> Create(CreateUserRequest request);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">修改用户请求</param>
    /// <returns>是否更新成功（昵称重复或用户不存在时返回 false）</returns>
    Task<bool> Update(long id, UpdateUserRequest request);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> Delete(long id);

    /// <summary>
    /// 批量删除用户
    /// </summary>
    /// <param name="ids">用户ID数组</param>
    /// <returns>实际删除的用户数量</returns>
    Task<int> BatchDelete(long[] ids);
}
