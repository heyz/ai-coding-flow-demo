#region  <<版本注释>>
/* ==============================================================================
// <copyright file="IUnitOfWorkManage.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：IUnitOfWorkManage
* 创 建 者：何应芝
* 创建时间：2026/5/25 15:44:08
* ==============================================================================*/
#endregion

using System.Reflection;

namespace SJ.BackEnd.Template.IRepository;

public interface IUnitOfWorkManage
{
    SqlSugarScope GetDbClient();
    int TranCount { get; }
    void BeginTran();
    void BeginTran(MethodInfo method);
    void CommitTran();
    void CommitTran(MethodInfo method);
    void RollbackTran();
    void RollbackTran(MethodInfo method);
}