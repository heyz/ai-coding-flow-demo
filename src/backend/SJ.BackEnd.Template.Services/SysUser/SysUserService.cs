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

using SJ.BackEnd.Template.Common.Extensions;
using SJ.BackEnd.Template.Model.Dtos.SysUser;

namespace SJ.BackEnd.Template.Services;

public class SysUserService(IBaseRepository<SysUser> repository) : BaseServices<SysUser>(repository), ISysUserService
{
    public async Task<PageModel<SysUser>> GetPagedList(int pageIndex, int pageSize, string? keyword)
    {
        Expression<Func<SysUser, bool>> whereExpression = _ => true;
        whereExpression = whereExpression.WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.RealName.Contains(keyword) || u.Nickname.Contains(keyword));

        string orderByFields = "Id desc";
        return await base.GetPagedListByExpression(whereExpression, pageIndex, pageSize, orderByFields);
    }

    public async Task<CreateUserResponse?> Create(CreateUserRequest request)
    {
        // 昵称唯一性校验
        if (!string.IsNullOrWhiteSpace(request.Nickname))
        {
            var exists = await IsNicknameExists(request.Nickname);
            if (exists)
                return null;
        }

        var user = new SysUser
        {
            Id = 0,
            Nickname = request.Nickname,
            RealName = request.RealName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            CreatedTime = DateTime.Now
        };

        var newId = await base.Insert(user);
        var created = await base.GetById(newId);

        return new CreateUserResponse
        {
            Id = created.Id,
            Nickname = created.Nickname,
            RealName = created.RealName,
            Gender = created.Gender,
            BirthDate = created.BirthDate,
            CreatedTime = created.CreatedTime
        };
    }

    public async Task<bool> Update(long id, UpdateUserRequest request)
    {
        // 昵称唯一性校验（排除当前用户）
        if (!string.IsNullOrWhiteSpace(request.Nickname))
        {
            var exists = await IsNicknameExists(request.Nickname, id);
            if (exists)
                return false;
        }

        var user = new SysUser
        {
            Id = id,
            Nickname = request.Nickname,
            RealName = request.RealName,
            Gender = request.Gender,
            BirthDate = request.BirthDate
        };

        return await base.Update(user);
    }

    public async Task<bool> Delete(long id)
    {
        return await base.DeleteById(id);
    }

    /// <summary>
    /// 检查昵称是否已存在
    /// </summary>
    /// <param name="nickname">昵称</param>
    /// <param name="excludeUserId">排除的用户ID（修改时排除自身）</param>
    /// <returns>昵称是否已被占用</returns>
    private async Task<bool> IsNicknameExists(string nickname, long? excludeUserId = null)
    {
        var users = await base.QueryByExpression(u => u.Nickname == nickname);
        if (excludeUserId.HasValue)
            return users.Any(u => u.Id != excludeUserId.Value);
        return users.Any();
    }
}
