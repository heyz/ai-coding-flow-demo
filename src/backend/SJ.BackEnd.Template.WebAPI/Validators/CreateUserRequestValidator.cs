#region  <<版本注释>>
/* ==============================================================================
// <copyright file="CreateUserRequestValidator.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：CreateUserRequestValidator
* 创 建 者：何应芝
* 创建时间：2026/6/8 18:00:00
* ==============================================================================*/
#endregion

using FluentValidation;
using SJ.BackEnd.Template.Model.Dtos.SysUser;

namespace SJ.BackEnd.Template.WebAPI.Validators;

/// <summary>
/// 创建用户请求 FluentValidation 验证器
/// 补充 Data Annotations 不方便表达的复杂验证规则
/// </summary>
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("出生日期不能晚于当前日期")
            .When(x => x.BirthDate.HasValue);
    }
}
