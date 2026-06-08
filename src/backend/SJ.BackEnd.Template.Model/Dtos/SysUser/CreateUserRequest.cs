#region  <<版本注释>>
/* ==============================================================================
// <copyright file="CreateUserRequest.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：CreateUserRequest
* 创 建 者：何应芝
* 创建时间：2026/6/8 16:30:00
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Model.Dtos.SysUser;

/// <summary>
/// 创建用户请求 DTO
/// </summary>
public class CreateUserRequest
{
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
    public int Gender { get; set; } = 0;

    /// <summary>
    /// 出生年月
    /// </summary>
    public DateTime? BirthDate { get; set; }
}
