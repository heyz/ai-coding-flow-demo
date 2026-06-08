#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ValidationFilter.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ValidationFilter
* 创 建 者：何应芝
* 创建时间：2026/6/8 18:00:00
* ==============================================================================*/
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SJ.BackEnd.Template.Model;

namespace SJ.BackEnd.Template.WebAPI;

/// <summary>
/// 全局模型验证过滤器
/// 替换 [ApiController] 默认的 ValidationProblemDetails 响应，
/// 统一使用 ApiResponse 格式返回验证错误
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorDict = new Dictionary<string, List<string>>();
            var errorSummaries = new List<string>();

            foreach (var (key, entry) in context.ModelState)
            {
                if (entry.Errors.Count == 0) continue;

                var fieldErrors = new List<string>();
                foreach (var error in entry.Errors)
                {
                    var msg = error.ErrorMessage;
                    if (string.IsNullOrEmpty(msg) && error.Exception != null)
                        msg = error.Exception.Message;
                    if (!string.IsNullOrEmpty(msg))
                    {
                        fieldErrors.Add(msg);
                        errorSummaries.Add($"{key}: {msg}");
                    }
                }

                if (fieldErrors.Count > 0)
                    errorDict[key] = fieldErrors;
            }

            var response = ApiResponse<ValidationErrorResponse>.Fail(
                errorDict.FirstOrDefault().Value.FirstOrDefault() ?? string.Empty,
                new ValidationErrorResponse { errors = errorDict }
            );
            response.status = 400;

            if (errorSummaries.Count > 0)
                response.msgDev = string.Join("; ", errorSummaries);

            context.Result = new ContentResult
            {
                Content = System.Text.Json.JsonSerializer.Serialize(response),
                StatusCode = 400,
                ContentType = "application/json"
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
