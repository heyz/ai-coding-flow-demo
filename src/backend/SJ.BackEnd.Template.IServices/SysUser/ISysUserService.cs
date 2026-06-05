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

namespace SJ.BackEnd.Template.IServices;

public interface ISysUserService
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
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetById(long id);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns>新记录的雪花ID</returns>
    Task<long> Create(SysUser user);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="user">用户信息</param>
    /// <returns>是否更新成功</returns>
    Task<bool> Update(long id, SysUser user);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> Delete(long id);
}
