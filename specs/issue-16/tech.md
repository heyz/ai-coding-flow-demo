# Tech Spec: 权限模块

**Issue:** #16
**Status:** Draft

---

## 1. 问题

需要实现 sys_permission 权限表的管理，支持树形层级结构和 CRUD 操作。

## 2. 相关代码

- `Model/Entities/SysRole.cs` — 参考实体模式
- `Services/SysRole/SysRoleService.cs` — 参考 Service 模式
- `WebAPI/Controllers/SysRoleController.cs` — 参考 Controller 模式

## 3. 当前状态

权限表不存在，需要新建实体和数据表。

## 4. 变更计划

### 4.1 新建实体

在 `Model/Entities/` 下新建 `SysPermission.cs`：

```csharp
[SugarTable("sys_permission")]
[Tenant("2")]
public class SysPermission
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(Length = 50)]
    public string Name { get; set; }

    [SugarColumn(Length = 100)]
    public string Code { get; set; }

    [SugarColumn(Length = 20)]
    public string Type { get; set; }  // menu / button / api

    public long ParentId { get; set; } = 0;

    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
```

### 4.2 创建 DTO

在 `Model/Dtos/SysPermission/` 下新建：

**CreatePermissionRequest.cs** + **UpdatePermissionRequest.cs**，均包含 name/code/type/parentId/path/icon/sortOrder 字段。

### 4.3 创建接口与实现

**ISysPermissionService**:  CRUD + GetTree
**SysPermissionService**:  实现（编码唯一性、子节点检查）

### 4.4 创建 Controller

路由 `[Route("permission")]`，5 个端点（增删改查 + 树形查询）。

### 4.5 CodeFirst

更新 `Program.cs` 的 InitTables 加入 `SysPermission`。

## 5. 风险和缓解措施

| 风险 | 缓解措施 |
|------|----------|
| 编码重复 | Service 层唯一性校验 |
| 删除父节点时子节点孤立 | 检查子节点，存在时禁止删除 |
| 循环引用 | parentId 不自引用自身 |

## 6. 测试

1. 创建权限 → 返回 200
2. 创建重复编码 → 返回 false
3. 删除父节点（有子节点）→ 返回 400
4. 删除叶子节点 → 返回 true
5. 树形查询 → 返回全部数据

## 7. 后续工作

- 权限与角色关联
- 前端动态菜单生成
