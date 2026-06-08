#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ValidationErrorResponse.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ValidationErrorResponse
* 创 建 者：何应芝
* 创建时间：2026/6/8 18:00:00
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Model;

/// <summary>
/// 验证错误详情
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// 字段名到错误消息数组的映射
    /// </summary>
    public Dictionary<string, List<string>> errors { get; set; } = new();
}
