# Tech Spec: 用户角色模块

**Issue:** #14
**Status:** Draft

---

## 1. 问题

需要实现用户与角色的多对多关联。SysUser 和 SysRole 实体已存在，但缺少关联表和关联操作。

## 2. 相关代码

- `Model/Entities/SysUser.cs` — 用户实体
- `Model/Entities/SysRole.cs` — 角色实体
- `IServices/SysUser/` — 用户服务接口
- `IServices/SysRole/` — 角色服务接口
- `WebAPI/Controllers/SysUserController.cs` — 参考 Controller 模式

## 3. 当前状态

- SysUser 和 SysRole 各自独立，无关联字段或关联表
- 用户管理 CRUD 已完成
- 角色管理 CRUD 已完成

## 4. 变更计划

### 4.1 新建关联实体

在 `Model/Entities/` 下新建 `SysUserRole.cs`：

```csharp
[SugarTable("sys_user_role")]
[Tenant("2")]
public class SysUserRole
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long RoleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

### 4.2 新建关联 DTO

在 `Model/Dtos/SysUserRole/` 下新建：

**BindUserRoleRequest.cs:**
```csharp
public class BindUserRoleRequest
{
    [Required] public long UserId { get; set; }
    [Required] public long RoleId { get; set; }
}
```

**UnbindUserRoleRequest.cs:** 同 BindUserRoleRequest。

### 4.3 新建 ISysUserRoleService 接口

```csharp
public interface ISysUserRoleService : IBaseServices<SysUserRole>
{
    Task<bool> Bind(long userId, long roleId);
    Task<bool> Unbind(long userId, long roleId);
    Task<List<SysRole>> GetRolesByUserId(long userId);
    Task<List<SysUser>> GetUsersByRoleId(long roleId);
}
```

### 4.4 新建 SysUserRoleService 实现

- **Bind**: 先检查是否已存在，不存在则插入
- **Unbind**: 按 userId + roleId 删除
- **GetRolesByUserId**: 通过关联表 join SysRole 查询
- **GetUsersByRoleId**: 通过关联表 join SysUser 查询

### 4.5 新建 SysUserRoleController

路由 `[Route("user-role")]`，注入 `ISysUserRoleService`。

### 4.6 CodeFirst

更新 Program.cs 的 `InitTables` 加入 `SysUserRole`。

### 4.7 数据流

```
绑定:   POST /user-role/bind { userId, roleId }
         → ValidationFilter → Controller → Service.Exists? → Insert

查询:   GET /user-role/user/{id}/roles
         → Controller → Service.Join查询 → 返回角色列表
```

## 5. 风险和缓解措施

| 风险 | 缓解措施 |
|------|----------|
| 重复绑定 | Bind 方法先查询是否存在 |
| 用户或角色不存在 | 调用前校验存在性 |
| 关联表数据一致 | SqlSugar 事务保证 |

## 6. 测试和验证

1. 绑定(合法) → 返回 true
2. 重复绑定 → 返回 false
3. 解绑 → 返回 true
4. 查询用户角色 → 返回正确列表
5. 查询角色用户 → 返回正确列表

## 7. 后续工作

- 在删除用户/角色时同时清除关联关系（级联删除）
