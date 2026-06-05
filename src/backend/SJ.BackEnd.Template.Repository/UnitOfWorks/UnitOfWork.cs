#region  <<版本注释>>
/* ==============================================================================
// <copyright file="UnitOfWork.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：UnitOfWork
* 创 建 者：何应芝
* 创建时间：2026/5/25 15:51:56
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Repository;

public class UnitOfWork : IDisposable
{
    public ILogger Logger { get; set; }
    public ISqlSugarClient Db { get; internal set; }

    public ITenant Tenant { get; internal set; }

    public bool IsTran { get; internal set; }

    public bool IsCommit { get; internal set; }

    public bool IsClose { get; internal set; }

    public void Dispose()
    {
        if (this.IsTran && !this.IsCommit)
        {
            Logger.LogDebug("UnitOfWork RollbackTran");
            this.Tenant.RollbackTran();
        }

        if (this.Db.Ado.Transaction != null || this.IsClose)
            return;
        this.Db.Close();
    }

    public bool Commit()
    {
        if (this.IsTran && !this.IsCommit)
        {
            Logger.LogDebug("UnitOfWork CommitTran");
            this.Tenant.CommitTran();
            this.IsCommit = true;
        }

        if (this.Db.Ado.Transaction == null && !this.IsClose)
        {
            this.Db.Close();
            this.IsClose = true;
        }

        return this.IsCommit;
    }
}