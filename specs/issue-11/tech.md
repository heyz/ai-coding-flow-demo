# Tech Spec: 批量删除用户接口

**Issue:** #11
**Status:** Draft

---

## 1. Problem

为 SysUserController 添加批量删除接口。当前只有单用户删除 `DELETE /sysuser/{id}`（调用 `base.DeleteById(id)`），客户端需要进行多次请求才能删除多个用户。需要新增一个接收 ID 列表的批量删除端点，返回实际删除数量。

## 2. Relevant Code

- `src/backend/SJ.BackEnd.Template.WebAPI/Controllers/SysUserController.cs:74` — 现有单用户删除 `[HttpDelete("{id}")]`
- `src/backend/SJ.BackEnd.Template.Services/SysUser/SysUserService.cs:91` — 现有 `Delete(long id)` 方法
- `src/backend/SJ.BackEnd.Template.Services/BaseService.cs:157` — 基类 `DeleteByIds(object[] ids)`，返回 `bool`
- `src/backend/SJ.BackEnd.Template.Repository/BASE/BaseRepository.cs:265` — 仓储层 `DeleteByIds`，底层调用 SqlSugar

## 3. Current State

- `SysUserController.Delete(long id)` 调用 `_sysUserService.Delete(id)` → `base.DeleteById(id)`，单用户级删除
- 已有 `base.DeleteByIds(object[] ids)` 方法，但返回 `bool` 而非删除数量
- `SysUserService` 继承了 `BaseServices<SysUser>`，可以通过 `Repository` 属性访问仓储层

## 4. Proposed Changes

### 4.1 新建批量删除请求 DTO

在 `SJ.BackEnd.Template.Model/Dtos/SysUser/` 下新建 `BatchDeleteRequest.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysUser;

public class BatchDeleteRequest
{
    [Required(ErrorMessage = "删除ID列表不能为空")]
    [MinLength(1, ErrorMessage = "删除ID列表不能为空")]
    public long[] ids { get; set; } = [];
}
```

### 4.2 在 ISysUserService 中添加接口方法

在 `ISysUserService` 中添加：

```csharp
Task<int> BatchDelete(long[] ids);
```

### 4.3 在 SysUserService 中添加实现

利用 `Repository` 直接调用 SqlSugar 的 `Deleteable` 来获取实际删除的行数：

```csharp
public async Task<int> BatchDelete(long[] ids)
{
    if (ids == null || ids.Length == 0) return 0;
    return await Repository.DeleteByIds(ids); // 或直接使用 Deleteable
}
```

权衡：`DeleteByIds` 返回 `bool`，需要改为返回 `int` 或在 Service 层使用 SqlSugar 的 `Deleteable().In(ids).ExecuteCommandAsync()` 获取受影响行数。

采用方案：在 SysUserService 中使用 `Repository` 的底层 DB 上下文调用 `Deleteable` 获取删除数量，或在现有 `DeleteByIds` 基础上封装。

### 4.4 在 SysUserController 中添加端点

```csharp
/// <summary>
/// 批量删除用户
/// </summary>
/// <param name="request">批量删除请求</param>
/// <returns>删除成功的数量</returns>
[HttpDelete("batch")]
public async Task<ApiResponse<int>> BatchDelete([FromBody] BatchDeleteRequest request)
{
    var count = await _sysUserService.BatchDelete(request.ids);
    return ApiResponse<int>.Success($"成功删除{count}条记录", count);
}
```

### 4.5 数据流

```
客户端 → DELETE /sysuser/batch { ids: [1,2,3] }
  → Model Binding + ValidationFilter 验证 BatchDeleteRequest
  → SysUserController.BatchDelete()
  → SysUserService.BatchDelete(long[] ids)
  → Repository.Deleteable<SysUser>().In(ids).ExecuteCommandAsync()
  ← 返回影响行数 (int)
  → ApiResponse<int>.Success("成功删除3条记录", 3)
```

## 5. Risks and Mitigations

| 风险 | 缓解措施 |
|------|----------|
| `DeleteByIds` 返回 `bool` 而非计数 | 在 Service 层使用 `Deleteable` 直接调用，或扩展仓储层方法 |
| 批量删除效率 | SqlSugar 的 `In` 子句生成一条 SQL，一次数据库交互即可完成 |
| 验证拦截 | `ValidationFilter` 自动处理 `[Required]` 和 `[MinLength]` 验证 |
| 与其他删除行为不一致 | 使用相同的物理删除逻辑 |

## 6. Testing and Validation

1. **空列表验证**：发送 `{ "ids": [] }`，确认返回 400
2. **正常批量删除**：发送 `{ "ids": [1, 2, 3] }`，确认返回 200 且 `response` = 删除数量
3. **不存在 ID**：发送 `{ "ids": [999] }`，确认返回 200 且 `response` = 0
4. **单删除不受影响**：`DELETE /sysuser/1` 仍然正常返回

## 7. Follow-ups

- 如果其他实体也需要批量删除，可将批量删除方法提取到 `BaseServices` 基类
- 考虑添加事务支持（当前非事务，每个删除独立）
