#region  <<版本注释>>
/* ==============================================================================
// <copyright file="CreateUserResponse.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：CreateUserResponse
* 创 建 者：何应芝
* 创建时间：2026/6/8 16:30:00
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Model.Dtos.SysUser;

/// <summary>
/// 创建用户响应 DTO
/// </summary>
public class CreateUserResponse
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 性别 (1-男, 2-女, 0-未知)
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 出生年月
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
}
