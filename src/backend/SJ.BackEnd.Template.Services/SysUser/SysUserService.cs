#region  <<版本注释>>
/* ==============================================================================
// <copyright file="SysUserService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：SysUserService
* 创 建 者：何应芝
* 创建时间：2026/6/5 16:30:00
* ==============================================================================*/
#endregion

using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;

namespace SJ.BackEnd.Template.Services;

public class SysUserService : BaseServices<SysUser>, ISysUserService
{
    public SysUserService(IBaseRepository<SysUser> repository) : base(repository)
    {
    }

    public async Task<PageModel<SysUser>> GetPagedList(int pageIndex, int pageSize, string? keyword)
    {
        Expression<Func<SysUser, bool>> whereExpression = _ => true;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            whereExpression = u => u.RealName.Contains(keyword) || u.Nickname.Contains(keyword);
        }

        string orderByFields = "Id desc";
        return await base.GetPagedListByExpression(whereExpression, pageIndex, pageSize, orderByFields);
    }

    public async Task<long> Create(SysUser user)
    {
        user.Id = 0;
        user.CreatedTime = DateTime.Now;
        return await base.Insert(user);
    }

    public async Task<bool> Update(long id, SysUser user)
    {
        user.Id = id;
        return await base.Update(user);
    }

    public async Task<bool> Delete(long id)
    {
        return await base.DeleteById(id);
    }
}
