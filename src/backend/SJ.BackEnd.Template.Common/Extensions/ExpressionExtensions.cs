#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ExpressionExtensions.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ExpressionExtensions
* 创 建 者：何应芝
* 创建时间：2026/6/8 20:00:00
* ==============================================================================*/
#endregion

using System.Linq.Expressions;

namespace SJ.BackEnd.Template.Common.Extensions;

/// <summary>
/// Expression 扩展方法
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 条件成立时追加表达式（AND 连接）
    /// 参考 SqlSugar 的 WhereIF 语义：仅当 condition 为 true 时应用 predicate
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="expr">原始表达式</param>
    /// <param name="condition">条件</param>
    /// <param name="predicate">要追加的表达式</param>
    /// <returns>组合后的表达式</returns>
    public static Expression<Func<T, bool>> WhereIF<T>(this Expression<Func<T, bool>> expr, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (!condition)
            return expr;

        var parameter = expr.Parameters[0];
        var body = Expression.AndAlso(expr.Body, ReplaceParameter(predicate.Body, predicate.Parameters[0], parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression source, ParameterExpression target)
    {
        return new ParameterReplacer(source, target).Visit(expression);
    }

    /// <summary>
    /// 表达式参数替换访问器
    /// </summary>
    private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        private readonly ParameterExpression _source = source;
        private readonly ParameterExpression _target = target;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }
}
