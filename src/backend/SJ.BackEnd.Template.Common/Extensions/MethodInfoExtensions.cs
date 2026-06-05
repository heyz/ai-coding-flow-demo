#region  <<版本注释>>
/* ==============================================================================
// <copyright file="MethodInfoExtensions.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：MethodInfoExtensions
* 创 建 者：何应芝
* 创建时间：2026/5/25 15:55:36
* ==============================================================================*/
#endregion

using System.Reflection;

namespace SJ.BackEnd.Template.Common;

public static class MethodInfoExtensions
{
    public static string GetFullName(this MethodInfo method)
    {
        if (method.DeclaringType == null)
        {
            return $@"{method.Name}";
        }

        return $"{method.DeclaringType.FullName}.{method.Name}";
    }
}
