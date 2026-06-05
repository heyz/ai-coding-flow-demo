#region  <<版本注释>>
/* ==============================================================================
// <copyright file="BaseService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：BaseService
* 创 建 者：何应芝
* 创建时间：2026/5/25 16:03:27
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Services;

public class BaseServices<TEntity>(IBaseRepository<TEntity> repository = default!) : IBaseServices<TEntity> where TEntity : class, new()
{
    public IBaseRepository<TEntity> Repository { get; set; } = repository;

    #region 查询

    public async Task<TEntity> GetById(object objId)
    {
        return await Repository.GetById(objId);
    }

    public async Task<TEntity> GetById(object objId, bool blnUseCache = false)
    {
        return await Repository.GetById(objId, blnUseCache);
    }

    public async Task<List<TEntity>> GetByIds(object[] lstIds)
    {
        return await Repository.GetByIds(lstIds);
    }

    public async Task<List<TEntity>> GetAll()
    {
        return await Repository.GetAll();
    }

    public async Task<List<TEntity>> QueryBySqlWhere(string where)
    {
        return await Repository.QueryBySqlWhere(where);
    }

    public async Task<List<TEntity>> QueryByExpression(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Repository.QueryByExpression(whereExpression);
    }

    public async Task<List<TResult>> Select<TResult>(Expression<Func<TEntity, TResult>> expression)
    {
        return await Repository.Select(expression);
    }

    public async Task<List<TResult>> SelectByCondition<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string orderByFields)
    {
        return await Repository.SelectByCondition(expression, whereExpression, orderByFields);
    }

    public async Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, string orderByFields)
    {
        return await Repository.QueryByExpressionOrdered(whereExpression, orderByFields);
    }

    public async Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
    {
        return await Repository.QueryByExpressionOrdered(whereExpression, orderByExpression, isAsc);
    }

    public async Task<List<TEntity>> QueryByWhereOrdered(string where, string orderByFields)
    {
        return await Repository.QueryByWhereOrdered(where, orderByFields);
    }

    public async Task<List<TEntity>> QueryTopNByExpression(Expression<Func<TEntity, bool>> whereExpression, int top, string orderByFields)
    {
        return await Repository.QueryTopNByExpression(whereExpression, top, orderByFields);
    }

    public async Task<List<TEntity>> QueryTopNByWhere(string where, int top, string orderByFields)
    {
        return await Repository.QueryTopNByWhere(where, top, orderByFields);
    }

    public async Task<List<TEntity>> QueryPagedByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string orderByFields)
    {
        return await Repository.QueryPagedByExpression(whereExpression, pageIndex, pageSize, orderByFields);
    }

    public async Task<List<TEntity>> QueryPagedByWhere(string where, int pageIndex, int pageSize, string orderByFields)
    {
        return await Repository.QueryPagedByWhere(where, pageIndex, pageSize, orderByFields);
    }

    public async Task<PageModel<TEntity>> GetPagedListByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 1, int pageSize = 20, string orderByFields = null)
    {
        return await Repository.GetPagedListByExpression(whereExpression, pageIndex, pageSize, orderByFields);
    }

    public async Task<List<TResult>> QueryThreeTableJoin<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
    {
        return await Repository.QueryThreeTableJoin(joinExpression, selectExpression, whereLambda);
    }

    #endregion

    #region 新增

    public async Task<long> Insert(TEntity model)
    {
        return await Repository.Insert(model);
    }

    public async Task<List<long>> InsertRange(List<TEntity> listEntity)
    {
        return await Repository.InsertRange(listEntity);
    }

    #endregion

    #region 更新

    public async Task<bool> Update(TEntity model)
    {
        return await Repository.Update(model);
    }

    public async Task<bool> UpdateRange(List<TEntity> models)
    {
        return await Repository.UpdateRange(models);
    }

    public async Task<bool> UpdateByAnonymousObject(object operateAnonymousObjects)
    {
        return await Repository.UpdateByAnonymousObject(operateAnonymousObjects);
    }

    public async Task<bool> UpdateBySelectedColumns(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string where = "")
    {
        return await Repository.UpdateBySelectedColumns(entity, lstColumns, lstIgnoreColumns, where);
    }

    #endregion

    #region 删除

    public async Task<bool> Delete(TEntity model)
    {
        return await Repository.Delete(model);
    }

    public async Task<bool> DeleteById(object id)
    {
        return await Repository.DeleteById(id);
    }

    public async Task<bool> DeleteByIds(object[] ids)
    {
        return await Repository.DeleteByIds(ids);
    }

    #endregion
}
