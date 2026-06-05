namespace SJ.BackEnd.Template.IRepository;

public interface IBaseRepository<TEntity> where TEntity : class
{
    #region 查询

    /// <summary>
    /// 根据主键查询单条实体
    /// </summary>
    /// <param name="objId">主键值（实体需标注 [SugarColumn(IsPrimaryKey=true)]，联合主键请使用 Where 条件查询）</param>
    /// <returns>实体对象，未找到时返回 null</returns>
    Task<TEntity> GetById(object objId);

    /// <summary>
    /// 根据主键查询单条实体（可选缓存）
    /// </summary>
    /// <param name="objId">主键值</param>
    /// <param name="blnUseCache">是否启用查询缓存，缓存时间默认 10 秒</param>
    /// <returns>实体对象，未找到时返回 null</returns>
    Task<TEntity> GetById(object objId, bool blnUseCache = false);

    /// <summary>
    /// 根据主键数组查询多条实体
    /// </summary>
    /// <param name="lstIds">主键值数组</param>
    /// <returns>符合条件的实体列表</returns>
    Task<List<TEntity>> GetByIds(object[] lstIds);

    /// <summary>
    /// 查询全部数据
    /// </summary>
    /// <returns>所有实体列表</returns>
    Task<List<TEntity>> GetAll();

    /// <summary>
    /// 使用 SQL WHERE 字符串条件查询
    /// </summary>
    /// <param name="where">SQL WHERE 条件字符串，如 "name = 'test' AND age > 18"</param>
    /// <returns>符合条件的实体列表</returns>
    Task<List<TEntity>> QueryBySqlWhere(string where);

    /// <summary>
    /// 使用 Lambda 表达式条件查询
    /// </summary>
    /// <param name="whereExpression">Lambda 过滤表达式，如 entity => entity.Name == "test"</param>
    /// <returns>符合条件的实体列表</returns>
    Task<List<TEntity>> QueryByExpression(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 使用 Lambda 表达式查询并投影为指定类型（SQL SELECT 投影）
    /// </summary>
    /// <typeparam name="TResult">投影目标类型</typeparam>
    /// <param name="expression">投影表达式，如 entity => new { entity.Id, entity.Name }</param>
    /// <returns>投影结果列表</returns>
    Task<List<TResult>> Select<TResult>(Expression<Func<TEntity, TResult>> expression);

    /// <summary>
    /// 使用 Lambda 表达式查询并投影，支持条件过滤和排序
    /// </summary>
    /// <typeparam name="TResult">投影目标类型</typeparam>
    /// <param name="expression">投影表达式，如 entity => new { entity.Id, entity.Name }</param>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>投影结果列表</returns>
    Task<List<TResult>> SelectByCondition<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string orderByFields);

    /// <summary>
    /// 使用 Lambda 表达式条件查询，并按指定字段排序
    /// </summary>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>排序后的实体列表</returns>
    Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, string orderByFields);

    /// <summary>
    /// 使用 Lambda 表达式条件查询，并按 Lambda 表达式排序
    /// </summary>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="orderByExpression">排序字段表达式，如 entity => entity.CreateTime</param>
    /// <param name="isAsc">是否升序，默认 true</param>
    /// <returns>排序后的实体列表</returns>
    Task<List<TEntity>> QueryByExpressionOrdered(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);

    /// <summary>
    /// 使用 SQL WHERE 字符串条件查询，并按指定字段排序
    /// </summary>
    /// <param name="where">SQL WHERE 条件字符串，如 "name = 'test'"</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>排序后的实体列表</returns>
    Task<List<TEntity>> QueryByWhereOrdered(string where, string orderByFields);

    /// <summary>
    /// 使用 Lambda 表达式条件查询前 N 条数据
    /// </summary>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="top">取前 N 条</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>前 N 条实体列表</returns>
    Task<List<TEntity>> QueryTopNByExpression(Expression<Func<TEntity, bool>> whereExpression, int top, string orderByFields);

    /// <summary>
    /// 使用 SQL WHERE 字符串条件查询前 N 条数据
    /// </summary>
    /// <param name="where">SQL WHERE 条件字符串</param>
    /// <param name="top">取前 N 条</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>前 N 条实体列表</returns>
    Task<List<TEntity>> QueryTopNByWhere(string where, int top, string orderByFields);

    /// <summary>
    /// 执行原生 SQL 语句查询
    /// </summary>
    /// <param name="sql">完整的 SQL 查询语句</param>
    /// <param name="parameters">SQL 参数，可选</param>
    /// <returns>查询结果实体列表</returns>
    Task<List<TEntity>> QueryByRawSql(string sql, SugarParameter[] parameters = null);

    /// <summary>
    /// 使用 Lambda 表达式条件分页查询（仅返回当前页数据列表）
    /// </summary>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="pageIndex">页码（从 0 开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>当前页实体列表（不含总记录数）</returns>
    Task<List<TEntity>> QueryPagedByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string orderByFields);

    /// <summary>
    /// 使用 SQL WHERE 字符串条件分页查询（仅返回当前页数据列表）
    /// </summary>
    /// <param name="where">SQL WHERE 条件字符串</param>
    /// <param name="pageIndex">页码（从 0 开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>当前页实体列表（不含总记录数）</returns>
    Task<List<TEntity>> QueryPagedByWhere(string where, int pageIndex, int pageSize, string orderByFields);

    /// <summary>
    /// 使用 Lambda 表达式条件分页查询（返回含分页信息的结果）
    /// </summary>
    /// <param name="whereExpression">过滤条件表达式</param>
    /// <param name="pageIndex">页码（从 1 开始）</param>
    /// <param name="pageSize">每页大小，默认 20</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>包含总记录数、页码和数据的分页结果</returns>
    Task<PageModel<TEntity>> GetPagedListByExpression(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 1, int pageSize = 20, string orderByFields = null);

    #endregion

    #region 多表联查

    /// <summary>
    /// 三表联合查询
    /// <para>
    /// 使用示例：
    /// <code>
    /// var result = await QueryThreeTableJoin&lt;Order, OrderDetail, Customer, OrderDto&gt;(
    ///     (o, d, c) => new object[] { JoinType.Left, o.Id == d.OrderId, JoinType.Left, o.CustomerId == c.Id },
    ///     (o, d, c) => new OrderDto { OrderName = o.Name, DetailName = d.Name, CustomerName = c.Name },
    ///     (o, d, c) => o.Status == 1
    /// );
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">主表实体类型</typeparam>
    /// <typeparam name="T2">关联表2实体类型</typeparam>
    /// <typeparam name="T3">关联表3实体类型</typeparam>
    /// <typeparam name="TResult">返回结果类型</typeparam>
    /// <param name="joinExpression">关联表达式，交替指定 JoinType 和 ON 条件，如 (o,d,c) => new object[] { JoinType.Left, o.Id == d.OrderId }</param>
    /// <param name="selectExpression">选择表达式，指定返回字段映射</param>
    /// <param name="whereLambda">过滤条件表达式，为 null 时不加过滤</param>
    /// <returns>查询结果列表</returns>
    Task<List<TResult>> QueryThreeTableJoin<T, T2, T3, TResult>(
        Expression<Func<T, T2, T3, object[]>> joinExpression,
        Expression<Func<T, T2, T3, TResult>> selectExpression,
        Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();

    /// <summary>
    /// 两表联合分页查询
    /// <para>
    /// 使用示例：
    /// <code>
    /// var result = await QueryTwoTableJoinPaged&lt;User, Role, UserDto&gt;(
    ///     (u, r) => new object[] { JoinType.Left, u.RoleId == r.Id },
    ///     (u, r) => new UserDto { UserName = u.Name, RoleName = r.Name },
    ///     dto => dto.RoleName != null,
    ///     pageIndex: 1,
    ///     pageSize: 10,
    ///     orderByFields: "u.Id desc"
    /// );
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">主表实体类型</typeparam>
    /// <typeparam name="T2">关联表实体类型</typeparam>
    /// <typeparam name="TResult">返回结果类型</typeparam>
    /// <param name="joinExpression">关联表达式，交替指定 JoinType 和 ON 条件</param>
    /// <param name="selectExpression">选择表达式，指定返回字段映射</param>
    /// <param name="whereExpression">对投影结果的过滤条件表达式</param>
    /// <param name="pageIndex">页码（从 1 开始）</param>
    /// <param name="pageSize">每页大小，默认 20</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>包含总记录数、页码和数据的分页结果</returns>
    Task<PageModel<TResult>> QueryTwoTableJoinPaged<T, T2, TResult>(
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<TResult, bool>> whereExpression,
        int pageIndex = 1,
        int pageSize = 20,
        string orderByFields = null);

    /// <summary>
    /// 两表联合分页分组查询
    /// <para>
    /// 使用示例：
    /// <code>
    /// var result = await QueryTwoTableJoinGroupedPaged&lt;Order, OrderDetail, GroupDto&gt;(
    ///     (o, d) => new object[] { JoinType.Left, o.Id == d.OrderId },
    ///     (o, d) => new GroupDto { CategoryId = o.CategoryId, Total = SqlFunc.AggregateCount(d.Id) },
    ///     dto => dto.CategoryId > 0,
    ///     o => o.CategoryId,
    ///     pageIndex: 1,
    ///     pageSize: 10
    /// );
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">主表实体类型</typeparam>
    /// <typeparam name="T2">关联表实体类型</typeparam>
    /// <typeparam name="TResult">返回结果类型</typeparam>
    /// <param name="joinExpression">关联表达式</param>
    /// <param name="selectExpression">选择表达式（可结合 SqlFunc.AggregateXxx 使用聚合函数）</param>
    /// <param name="whereExpression">对投影结果的过滤条件表达式</param>
    /// <param name="groupExpression">分组表达式，如 o => o.CategoryId</param>
    /// <param name="pageIndex">页码（从 1 开始）</param>
    /// <param name="pageSize">每页大小，默认 20</param>
    /// <param name="orderByFields">排序字段，如 "name asc, age desc"</param>
    /// <returns>包含总记录数、页码和数据的分页结果</returns>
    Task<PageModel<TResult>> QueryTwoTableJoinGroupedPaged<T, T2, TResult>(
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<TResult, bool>> whereExpression,
        Expression<Func<T, object>> groupExpression,
        int pageIndex = 1,
        int pageSize = 20,
        string orderByFields = default!);

    #endregion

    #region 新增

    /// <summary>
    /// 插入单条实体，返回雪花ID
    /// </summary>
    /// <param name="model">要插入的实体对象</param>
    /// <returns>新记录的雪花ID</returns>
    Task<long> Insert(TEntity model);

    /// <summary>
    /// 插入单条实体（指定插入列），返回雪花ID
    /// </summary>
    /// <param name="model">要插入的实体对象</param>
    /// <param name="insertColumns">指定只插入的列表达式，如 entity => new { entity.Name, entity.Age }，为 null 时插入所有列</param>
    /// <returns>新记录的雪花ID</returns>
    Task<long> InsertByColumns(TEntity model, Expression<Func<TEntity, object>> insertColumns = null);

    /// <summary>
    /// 批量插入实体，返回雪花ID列表
    /// </summary>
    /// <param name="listEntity">要插入的实体集合</param>
    /// <returns>新记录的雪花ID列表</returns>
    Task<List<long>> InsertRange(List<TEntity> listEntity);

    #endregion

    #region 更新

    /// <summary>
    /// 根据主键更新实体（以实体主键值为定位条件）
    /// </summary>
    /// <param name="model">包含主键的更新实体</param>
    /// <returns>是否更新成功</returns>
    Task<bool> Update(TEntity model);

    /// <summary>
    /// 根据主键批量更新实体
    /// </summary>
    /// <param name="models">包含主键的更新实体集合</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateRange(List<TEntity> models);

    /// <summary>
    /// 根据自定义 SQL WHERE 条件更新实体
    /// </summary>
    /// <param name="model">要更新的实体</param>
    /// <param name="where">SQL WHERE 条件字符串，如 "id > 10"</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateByCondition(TEntity model, string where);

    /// <summary>
    /// 使用匿名对象更新指定列（以匿名对象中的属性名为列名，主键属性作为定位条件）
    /// <para>
    /// 使用示例：
    /// <code>
    /// await UpdateByAnonymousObject(new { Id = 1, Name = "张三" });
    /// // 将更新 Id=1 记录的 Name 为 "张三"
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="operateAnonymousObjects">匿名对象，必须包含主键字段</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateByAnonymousObject(object operateAnonymousObjects);

    /// <summary>
    /// 根据主键更新实体，可指定更新列、忽略列和附加条件
    /// </summary>
    /// <param name="entity">包含主键的更新实体</param>
    /// <param name="lstColumns">只更新这些列，为 null 时更新所有列</param>
    /// <param name="lstIgnoreColumns">忽略这些列不更新</param>
    /// <param name="where">附加 SQL WHERE 条件字符串</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateBySelectedColumns(TEntity entity, List<string> lstColumns = default!, List<string> lstIgnoreColumns = default!, string where = "");

    #endregion

    #region 删除

    /// <summary>
    /// 根据实体主键删除
    /// </summary>
    /// <param name="model">包含主键的实体对象</param>
    /// <returns>是否删除成功</returns>
    Task<bool> Delete(TEntity model);

    /// <summary>
    /// 根据主键 ID 删除
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteById(object id);

    /// <summary>
    /// 根据主键 ID 数组批量删除
    /// </summary>
    /// <param name="ids">主键值数组</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteByIds(object[] ids);

    #endregion
}
