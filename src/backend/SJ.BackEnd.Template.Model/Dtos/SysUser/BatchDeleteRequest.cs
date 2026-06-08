#region  <<版本注释>>
/* ==============================================================================
// <copyright file="BatchDeleteRequest.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：BatchDeleteRequest
* 创 建 者：何应芝
* 创建时间：2026/6/8 21:00:00
* ==============================================================================*/
#endregion

using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysUser;

/// <summary>
/// 批量删除用户请求 DTO
/// </summary>
public class BatchDeleteRequest
{
    /// <summary>
    /// 要删除的用户 ID 列表
    /// </summary>
    [Required(ErrorMessage = "删除ID列表不能为空")]
    [MinLength(1, ErrorMessage = "删除ID列表不能为空")]
    public long[] ids { get; set; } = [];
}
