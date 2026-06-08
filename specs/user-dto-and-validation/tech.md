# Tech Spec: 用户 DTO 与昵称唯一性校验

## 当前架构

```
WebAPI/SysUserController → IServices/ISysUserService → Services/SysUserService → BaseServices<SysUser>
                                                                                     ↓
                                                                              IBaseRepository<SysUser>
```

Controller 直接接收 `SysUser` 实体作为 `[FromBody]` 参数，Service 直接操作实体。

## 目标架构

```
WebAPI/SysUserController → IServices/ISysUserService → Services/SysUserService → BaseServices<SysUser>
     ↓ (接收 DTO)              ↓ (DTO 参数)               ↓ (校验 + 映射)          ↓
CreateUserRequest          Create(CreateUserRequest)    昵称重复检查           IBaseRepository<SysUser>
UpdateUserRequest          Update(long, UpdateUserRequest)  DTO → Entity 映射
                           (返回 DTO)                   Entity → DTO 映射
```

## 文件变更清单

### 新增文件

| 文件 | 位置 | 说明 |
|------|------|------|
| `CreateUserRequest.cs` | `Model/Dtos/SysUser/` | 创建用户请求 DTO |
| `UpdateUserRequest.cs` | `Model/Dtos/SysUser/` | 修改用户请求 DTO |
| `CreateUserResponse.cs` | `Model/Dtos/SysUser/` | 创建用户响应 DTO |

### 修改文件

| 文件 | 变更内容 |
|------|---------|
| `IServices/SysUser/ISysUserService.cs` | `Create` 参数改为 `CreateUserRequest`，返回 `CreateUserResponse`；`Update` 参数改为 `UpdateUserRequest` |
| `Services/SysUser/SysUserService.cs` | 实现昵称唯一性校验逻辑；DTO ↔ Entity 映射 |
| `WebAPI/Controllers/SysUserController.cs` | `Create`/`Update` 接口使用 DTO 参数和响应类型 |

## 实现计划

### Step 1: 创建 DTO 类

在 `Model/Dtos/SysUser/` 下创建三个 DTO：

```csharp
// CreateUserRequest.cs
public class CreateUserRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int Gender { get; set; } = 0;
    public DateTime? BirthDate { get; set; }
}

// UpdateUserRequest.cs  (结构相同，独立类型)
public class UpdateUserRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int Gender { get; set; } = 0;
    public DateTime? BirthDate { get; set; }
}

// CreateUserResponse.cs
public class CreateUserResponse
{
    public long Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedTime { get; set; }
}
```

### Step 2: 更新 ISysUserService

- `Create(SysUser user) → Task<long>` 改为 `Create(CreateUserRequest request) → Task<CreateUserResponse>`
- `Update(long id, SysUser user) → Task<bool>` 改为 `Update(long id, UpdateUserRequest request) → Task<bool>`
- 新增 import: `using SJ.BackEnd.Template.Model.Dtos.SysUser;`

### Step 3: 实现 SysUserService

**昵称唯一性校验逻辑：**

```csharp
private async Task<bool> IsNicknameExists(string nickname, long? excludeUserId = null)
{
    if (string.IsNullOrWhiteSpace(nickname))
        return false;  // 空昵称不校验

    var users = await base.QueryByExpression(u => u.Nickname == nickname);
    if (excludeUserId.HasValue)
        return users.Any(u => u.Id != excludeUserId.Value);
    return users.Any();
}
```

**Create 方法：**
1. 校验昵称唯一性 → 存在则抛异常或返回失败标识
2. DTO → SysUser 实体映射
3. 设置 Id=0, CreatedTime=DateTime.Now
4. 调用 `base.Insert(user)` 获取新 ID
5. 查询刚插入的完整实体
6. 实体 → CreateUserResponse 映射返回

**Update 方法：**
1. 校验昵称唯一性（排除当前用户）→ 存在则返回 false
2. DTO → SysUser 实体映射
3. 设置 Id = 目标ID
4. 调用 `base.Update(user)` 返回结果

> **错误返回策略**：当前项目没有全局异常处理中间件，且 Service 层返回 `bool`/`long` 等简单类型。
> 昵称重复是一种**业务校验失败**，通过返回值表达。`Create` 返回 `null` 表示昵称重复，
> `Update` 返回 `false` 表示昵称重复或用户不存在，Controller 层根据返回值决定返回
> `ApiResponse.Fail("用户昵称已存在")`。

### Step 4: 更新 SysUserController

**Create 接口：**
```csharp
[HttpPost]
public async Task<ApiResponse<CreateUserResponse>> Create([FromBody] CreateUserRequest request)
{
    var result = await _sysUserService.Create(request);
    if (result == null)
        return ApiResponse<CreateUserResponse>.Fail("用户昵称已存在");
    return ApiResponse<CreateUserResponse>.Success("创建成功", result);
}
```

**Update 接口：**
```csharp
[HttpPut("{id}")]
public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateUserRequest request)
{
    var result = await _sysUserService.Update(id, request);
    if (!result)
        return ApiResponse<bool>.Fail("更新失败");
    return ApiResponse<bool>.Success("更新成功", true);
}
```

> 注意：当前 Update 返回 `ApiResponse<bool>.Fail("更新失败")` 时无法区分"昵称重复"还是"用户不存在"。
> 这里做最小实现：昵称重复 → `ApiResponse<bool>.Fail("用户昵称已存在")`；
> 用户不存在 → `ApiResponse<bool>.Fail("更新失败")`。

## 数据流

### 创建用户流程

```
Client → POST /SysUser { CreateUserRequest }
  → Controller.Create(request)
    → Service.Create(request)
      → 昵称非空 → QueryByExpression(n => n.Nickname == request.Nickname)
        → Any() → return null (昵称已存在)
      → DTO → SysUser { Id=0, CreatedTime=now, ... }
      → base.Insert(entity) → long newId
      → base.GetById(newId) → SysUser entity
      → SysUser → CreateUserResponse
      → return CreateUserResponse
    → if null → ApiResponse.Fail("用户昵称已存在")
    → ApiResponse.Success("创建成功", response)
```

### 修改用户流程

```
Client → PUT /SysUser/123 { UpdateUserRequest }
  → Controller.Update(123, request)
    → Service.Update(123, request)
      → 昵称非空 → QueryByExpression(n => n.Nickname == request.Nickname)
        → Any(u => u.Id != 123) → return false (昵称被其他用户占用)
      → DTO → SysUser { Id=123, ... }
      → base.Update(entity) → bool
      → return result
    → if false && 昵称重复 → ApiResponse.Fail("用户昵称已存在")
    → if false && 其他原因 → ApiResponse.Fail("更新失败")
    → ApiResponse.Success("更新成功", true)
```

## 风险与缓解

| 风险 | 缓解 |
|------|------|
| 昵称校验存在并发窗口（检查→插入之间另一请求插入了相同昵称） | 当前项目规模接受此风险；后续可在数据库层添加唯一索引 |
| DTO 与实体字段不一致 | Mapster 映射可减少手动赋值错误；当前手动映射更显式、可读 |
| Service 返回 null 表示业务失败不够语义化 | 保持与项目现有模式一致（返回简单类型）；后续可引入 Result<T> 模式 |

## 验证计划

1. `dotnet build` 编译零错误
2. 手动测试：创建用户 → 创建同名昵称用户 → 期望 "用户昵称已存在"
3. 手动测试：修改用户昵称 → 改为已有昵称 → 期望 "用户昵称已存在"
4. 手动测试：修改用户 → 不改昵称 → 期望成功
5. 手动测试：创建空昵称用户 → 期望成功

## 后续技术债

- 数据库 `sys_user.Nickname` 列添加唯一索引
- 引入 `Result<T>` 或 `OperationResult` 模式替代 null/bool 返回语义
- 考虑全局异常处理中间件统一错误响应格式
