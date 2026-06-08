#region  <<版本注释>>
/* ==============================================================================
// <copyright file="BaseRepository.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：BaseRepository
* 创 建 者：何应芝
* 创建时间：2026/5/25 15:38:25
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Repository;

public class BaseRepository<TEntity>(IUnitOfWorkManage uowMgr) : IBaseRepository<TEntity> where TEntity : class, new()
{
    private readonly IUnitOfWorkManage _uowMgr = uowMgr;
    private readonly SqlSugarScope _dbScope = uowMgr.GetDbClient();

    private ISqlSugarClient _db
    {
        get
        {
            return _dbScope;
        }
    }

    #region 查询

    public async Task<TEntity> GetById(object objId)
    {
        return await _db.Queryable<TEntity>().In(objId).SingleAsync();
    }

    public async Task<TEntity> GetById(object objId, bool blnUseCache = false)
    {
        return await _db.Queryable<TEntity>().WithCacheIF(blnUseCache, 10).In(objId).SingleAsync();
    }

    public async Task<List<TEntity>> GetByIds(object[] lstIds)
    {
        return await _db.Queryable<TEntity>().In(lstIds).ToListAsync();
    }

    public async Task<List<TEntity>> GetAll()
    {
        return await _db.Queryable<TEntity>().ToListAsync();
    }

    public async Task<List<TEntity>> QueryBySqlWhere(string where)
    {
        return await _db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(where), where).ToListAsync();
    }

    public async Task<List<TEntity>> QueryByExpression(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
    }

    public async Task<List<TResult>> Select<TResult>(Expression<Func<TEntity, TResult>> expression)
    {
        return await _db.Queryable<TEntity>().Select(expression).ToListAsync();
    }

    public async Task<List<TResult>> SelectByCondition<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields).WhereIF(whereExpression != null, whereExpression).Select(expression).ToListAsync();
    }

    public async Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, string orderByFields)
    {
        return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByFields != null, orderByFields).ToListAsync();
    }

    public async Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
    {
        return await _db.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToListAsync();
    }

    public async Task<List<TEntity>> QueryByWhereOrdered(string where, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields).WhereIF(!string.IsNullOrEmpty(where), where).ToListAsync();
    }

    public async Task<List<TEntity>> QueryTopNByExpression(Expression<Func<TEntity, bool>> whereExpression, int top, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields).WhereIF(whereExpression != null, whereExpression).Take(top).ToListAsync();
    }

    public async Task<List<TEntity>> QueryTopNByWhere(string where, int top, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields).WhereIF(!string.IsNullOrEmpty(where), where).Take(top).ToListAsync();
    }

    public async Task<List<TEntity>> QueryByRawSql(string sql, SugarParameter[] parameters = default!)
    {
        return await _db.Ado.SqlQueryAsync<TEntity>(sql, parameters);
    }

    public async Task<List<TEntity>> QueryPagedByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields)
            .WhereIF(whereExpression != null, whereExpression).ToPageListAsync(pageIndex, pageSize);
    }

    public async Task<List<TEntity>> QueryPagedByWhere(string where, int pageIndex, int pageSize, string orderByFields)
    {
        return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields)
            .WhereIF(!string.IsNullOrEmpty(where), where).ToPageListAsync(pageIndex, pageSize);
    }

    public async Task<PageModel<TEntity>> GetPagedListByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 1, int pageSize = 20, string orderByFields = null)
    {
        RefAsync<int> totalCount = 0;
        var list = await _db.Queryable<TEntity>()
            .OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields)
            .WhereIF(whereExpression != null, whereExpression)
            .ToPageListAsync(pageIndex, pageSize, totalCount);

        return new PageModel<TEntity>(pageIndex, totalCount, pageSize, list);
    }

    #endregion

    #region 多表联查

    public async Task<List<TResult>> QueryThreeTableJoin<T, T2, T3, TResult>(
        Expression<Func<T, T2, T3, object[]>> joinExpression,
        Expression<Func<T, T2, T3, TResult>> selectExpression,
        Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
    {
        if (whereLambda == null)
        {
            return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
        }

        return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
    }

    public async Task<PageModel<TResult>> QueryTwoTableJoinPaged<T, T2, TResult>(
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<TResult, bool>> whereExpression,
        int pageIndex = 1,
        int pageSize = 20,
        string orderByFields = null)
    {
        RefAsync<int> totalCount = 0;
        var list = await _db.Queryable<T, T2>(joinExpression)
            .Select(selectExpression)
            .OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields)
            .WhereIF(whereExpression != null, whereExpression)
            .ToPageListAsync(pageIndex, pageSize, totalCount);
        return new PageModel<TResult>(pageIndex, totalCount, pageSize, list);
    }

    public async Task<PageModel<TResult>> QueryTwoTableJoinGroupedPaged<T, T2, TResult>(
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<TResult, bool>> whereExpression,
        Expression<Func<T, object>> groupExpression,
        int pageIndex = 1,
        int pageSize = 20,
        string orderByFields = default!)
    {
        RefAsync<int> totalCount = 0;
        var list = await _db.Queryable<T, T2>(joinExpression).GroupBy(groupExpression)
            .Select(selectExpression)
            .OrderByIF(!string.IsNullOrEmpty(orderByFields), orderByFields)
            .WhereIF(whereExpression != null, whereExpression)
            .ToPageListAsync(pageIndex, pageSize, totalCount);
        return new PageModel<TResult>(pageIndex, totalCount, pageSize, list);
    }

    #endregion

    #region 新增

    public async Task<long> Insert(TEntity entity)
    {
        var insert = _db.Insertable(entity);
        return await insert.ExecuteReturnSnowflakeIdAsync();
    }

    public async Task<long> InsertByColumns(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null)
    {
        var insert = _db.Insertable(entity);
        if (insertColumns == null)
        {
            return await insert.ExecuteReturnSnowflakeIdAsync();
        }
        else
        {
            return await insert.InsertColumns(insertColumns).ExecuteReturnSnowflakeIdAsync();
        }
    }

    public async Task<List<long>> InsertRange(List<TEntity> listEntity)
    {
        return await _db.Insertable(listEntity.ToArray()).ExecuteReturnSnowflakeIdListAsync();
    }

    #endregion

    #region 更新

    public async Task<bool> Update(TEntity entity)
    {
        return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> UpdateRange(List<TEntity> entity)
    {
        return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> UpdateByCondition(TEntity entity, string where)
    {
        return await _db.Updateable(entity).Where(where).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> UpdateByAnonymousObject(object operateAnonymousObjects)
    {
        return await _db.Updateable<TEntity>(operateAnonymousObjects).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> UpdateBySelectedColumns(
        TEntity entity,
        List<string> lstColumns = null,
        List<string> lstIgnoreColumns = null,
        string where = "")
    {
        IUpdateable<TEntity> up = _db.Updateable(entity);
        if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
        {
            up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
        }

        if (lstColumns != null && lstColumns.Count > 0)
        {
            up = up.UpdateColumns(lstColumns.ToArray());
        }

        if (!string.IsNullOrEmpty(where))
        {
            up = up.Where(where);
        }

        return await up.ExecuteCommandHasChangeAsync();
    }

    #endregion

    #region 删除

    public async Task<bool> Delete(TEntity entity)
    {
        return await _db.Deleteable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteById(object id)
    {
        return await _db.Deleteable<TEntity>().In(id).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteByIds(object[] ids)
    {
        return await _db.Deleteable<TEntity>().In(ids).ExecuteCommandHasChangeAsync();
    }

    public async Task<int> DeleteByIdsReturnCount(object[] ids)
    {
        return await _db.Deleteable<TEntity>().In(ids).ExecuteCommandAsync();
    }

    #endregion
}
