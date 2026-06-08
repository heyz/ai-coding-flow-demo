# Tech Spec: 角色管理模块

**Issue:** #13
**Status:** Draft

---

## 1. 问题

角色实体（SysRole）已存在，但缺少 CRUD 接口和业务逻辑。需要新增完整的角色管理 API。

## 2. 相关代码

- `src/backend/SJ.BackEnd.Template.Model/Entities/SysRole.cs` — 现有角色实体
- `src/backend/SJ.BackEnd.Template.WebAPI/Controllers/SysUserController.cs` — 参考 Controller 模式
- `src/backend/SJ.BackEnd.Template.Services/SysUser/SysUserService.cs` — 参考 Service 模式
- `src/backend/SJ.BackEnd.Template.IServices/SysUser/ISysUserService.cs` — 参考接口模式
- `src/backend/SJ.BackEnd.Template.Model/Dtos/SysUser/` — 参考 DTO 模式

## 3. 当前状态

- SysRole 实体存在（Id, Name, Code, Description, IsSystem, SortOrder, CreatedAt, UpdatedAt）
- 无角色相关的 DTO、Service、Controller
- SysUser 实体没有 roleId 字段，无法判断角色与用户的关联关系

## 4. 变更计划

### 4.1 创建 DTO

在 `Model/Dtos/SysRole/` 下新建：

**CreateRoleRequest.cs:**
```csharp
public class CreateRoleRequest
{
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, ErrorMessage = "角色名称长度不能超过{1}个字符")]
    public string Name { get; set; }

    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(50, ErrorMessage = "角色编码长度不能超过{1}个字符")]
    public string Code { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public int SortOrder { get; set; } = 0;
}
```

**UpdateRoleRequest:** 同 CreateRoleRequest。

### 4.2 创建 ISysRoleService 接口

```csharp
public interface ISysRoleService : IBaseServices<SysRole>
{
    Task<PageModel<SysRole>> GetPagedList(int pageIndex, int pageSize, string? keyword);
    Task<SysRole?> Create(CreateRoleRequest request);
    Task<bool> Update(long id, UpdateRoleRequest request);
    Task<bool> Delete(long id);
}
```

### 4.3 创建 SysRoleService 实现

- 创建：使用 `base.Insert()`，名称重复时返回 null
- 更新：使用 `base.Update()`，名称重复时返回 false
- 删除：先检查 `IsSystem` 标记，系统角色不可删除；暂无法校验用户关联
- 分页查询：使用 `base.GetPagedListByExpression()` + WhereIF 关键词搜索

**关于用户关联检查：** 当前 SysUser 实体无 roleId 字段，且无角色-用户关联表。删除检查暂不可实现，在接口文档中说明。后续添加关联关系后可补充。

### 4.4 创建 SysRoleController

遵循 SysUserController 模式，路由 `[Route("[controller]")]`，注入 `ISysRoleService`。

### 4.5 数据流

```
客户端 → POST/PUT/DELETE/GET /role
  → ValidationFilter 验证
  → SysRoleController
  → SysRoleService
  → BaseServices<SysRole> / BaseRepository<SysRole>
  → SqlSugar CRUD
```

## 5. 风险和缓解措施

| 风险 | 缓解措施 |
|------|----------|
| 名称编码唯一性 | Service 层在创建/更新时校验重复 |
| 系统角色保护 | `IsSystem = true` 的角色拒绝删除 |
| 用户关联检查 | 暂无法实现，SysUser 无 roleId 字段 |

## 6. 测试和验证

1. 创建角色（合法数据）— 返回 200
2. 创建角色（空名称）— 返回 400
3. 创建角色（重复名称）— 返回 success: false
4. 更新角色 — 返回 200
5. 删除角色（非系统角色）— 返回 200
6. 删除角色（系统角色）— 返回 400
7. 获取单个角色 — 返回角色数据
8. 分页查询 + 关键词搜索 — 返回正确分页

## 7. 后续工作

- 在 SysUser 实体添加 roleId 字段或创建角色-用户关联表后，补充删除检查逻辑
