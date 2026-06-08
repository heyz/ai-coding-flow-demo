# Product Spec: 批量删除用户接口

**Issue:** #11
**Status:** Draft

Figma: none provided

---

## 1. Summary

为 SysUserController 添加一个批量删除接口，允许客户端通过一次 HTTP 请求删除多个用户，返回实际删除的用户数量。

## 2. Problem

当前系统仅支持单用户删除（`DELETE /sysuser/{id}`）。当需要清理大量用户（如数据迁移、批量注销、测试数据清理）时，客户端需要逐条发送请求，效率低且无法保证原子性。

## 3. Goals

1. 提供一个 DELETE 接口接受用户 ID 数组，一次性删除多个用户
2. 返回实际删除成功的用户数量
3. 对于不存在或已被删除的 ID，静默跳过（不报错）
4. 与现有删除行为一致（物理删除）
5. 遵循现有 `ApiResponse<T>` 响应格式

## 4. Non-goals

- 不实现软删除（当前系统已是物理删除，保持一致）
- 不实现异步或后台任务
- 不实现事务性批量删除（每个删除独立）
- 不接受 CSV、文件上传等其他批量输入格式

## 5. User Experience

### 请求格式

```
DELETE /sysuser/batch
Content-Type: application/json

{
  "ids": [1, 2, 3]
}
```

### 响应格式（成功）

```json
{
  "status": 200,
  "success": true,
  "msg": "成功删除3条记录",
  "response": 3
}
```

### 行为规则

1. **空 ID 列表**：`ids` 为空数组时，返回 400 和错误信息 "删除ID列表不能为空"
2. **部分 ID 不存在**：不存在的 ID 被静默忽略，返回值只包含实际删除的记录数
3. **混合场景**：输入 `[1, 2, 999]`，如果 ID 1 和 2 存在、999 不存在，返回 `success: true, response: 2`
4. **响应字段**：`response` 字段返回 `int`（删除成功数量），使用 `ApiResponse<int>` 类型
5. **验证**：`ids` 字段使用 `[Required]` 和 `[MinLength(1)]` 验证，空请求体或无 ids 字段时返回验证错误

## 6. Success Criteria

1. 发送 `DELETE /sysuser/batch` 带 `{ "ids": [1, 2, 3] }`，三个用户均被删除，返回 `response: 3`
2. 发送 `DELETE /sysuser/batch` 带 `{ "ids": [] }`，返回 400
3. 发送 `DELETE /sysuser/batch` 带 `{ "ids": [999] }`（不存在），返回 `response: 0`
4. 发送 `DELETE /sysuser/batch` 不带 body，返回 400 验证错误
5. 单用户删除接口 `DELETE /sysuser/{id}` 不受影响

## 7. Validation

1. **curl 测试**：分别发送上述合法、空列表、不存在 ID 的请求，验证状态码和响应格式
2. **兼容性测试**：验证 `DELETE /sysuser/{id}` 仍然正常工作

## 8. Open Questions

- 是否需要事务性批量删除（全部成功或全部回滚）？当前假设不需要，每个删除独立。
