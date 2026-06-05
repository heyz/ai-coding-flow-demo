#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ApiResponse.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ApiResponse
* 创 建 者：何应芝
* 创建时间：2026/5/29 14:53:10
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Model;

/// <summary>
/// 通用返回信息类
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int status { get; set; } = 200;
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool success { get; set; } = false;
    /// <summary>
    /// 返回信息
    /// </summary>
    public string msg { get; set; } = string.Empty;
    /// <summary>
    /// 开发者信息
    /// </summary>
    public string msgDev { get; set; } = string.Empty;
    /// <summary>
    /// 返回数据集合
    /// </summary>
    public T? response { get; set; }

    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static ApiResponse<T> Success(string msg)
    {
        return Message(true, msg, default!);
    }
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="response">数据</param>
    /// <returns></returns>
    public static ApiResponse<T> Success(string msg, T response)
    {
        return Message(true, msg, response);
    }
    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static ApiResponse<T> Fail(string msg)
    {
        return Message(false, msg, default!);
    }
    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="response">数据</param>
    /// <returns></returns>
    public static ApiResponse<T> Fail(string msg, T response)
    {
        return Message(false, msg, response);
    }
    /// <summary>
    /// 返回消息
    /// </summary>
    /// <param name="success">失败/成功</param>
    /// <param name="msg">消息</param>
    /// <param name="response">数据</param>
    /// <returns></returns>
    public static ApiResponse<T> Message(bool success, string msg, T response)
    {
        return new ApiResponse<T>() { msg = msg, response = response, success = success };
    }
}

public class ApiResponse
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int status { get; set; } = 200;
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool success { get; set; } = false;
    /// <summary>
    /// 返回信息
    /// </summary>
    public string msg { get; set; } = "";
    /// <summary>
    /// 返回数据集合
    /// </summary>
    public object? response { get; set; }
}