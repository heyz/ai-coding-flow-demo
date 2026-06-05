#region  <<版本注释>>
/* ==============================================================================
// <copyright file="GlobalExceptionsFilter.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：GlobalExceptionsFilter
* 创 建 者：何应芝
* 创建时间：2026/5/29 14:51:01
* ==============================================================================*/
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SJ.BackEnd.Template.Model;
using SqlSugar.Extensions;

namespace SJ.BackEnd.Template.WebAPI;

/// <summary>
/// 全局异常错误日志
/// </summary>
public class GlobalExceptionsFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> logger) : IExceptionFilter
{
    private readonly IWebHostEnvironment _env = env;
    private readonly ILogger<GlobalExceptionsFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var json = new ApiResponse<string>
        {
            msg = context.Exception.Message,//错误信息
            status = 500//500异常 
        };

        if (_env.EnvironmentName.ObjToString().Equals("Development"))
        {
            json.msgDev = context.Exception.StackTrace ?? string.Empty;//堆栈信息
        }

        context.Result = new ContentResult
        {
            Content = System.Text.Json.JsonSerializer.Serialize(json)
        };

        //进行错误日志记录
        _logger.LogError(json.msg + WriteLog(json.msg, context.Exception));
    }

    /// <summary>
    /// 自定义返回格式
    /// </summary>
    /// <param name="throwMsg"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public string WriteLog(string throwMsg, Exception ex)
    {
        return string.Format("\r\n【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}", [ throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace ]);
    }

}
