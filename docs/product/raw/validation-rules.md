# 验证规则

## 全局验证机制

所有 API 请求参数在进入 Controller Action 之前由 `ValidationFilter` 自动验证。验证失败返回统一的 `ApiResponse<T>` 格式（HTTP 400）。

验证支持两种方式：
- **Data Annotations** — 声明在 DTO 属性上（`[Required]`、`[StringLength]`、`[Range]` 等）
- **FluentValidation** — 独立的 Validator 类，处理复杂业务规则（如日期不能晚于当天）

两种验证的错误会合并到同一个响应中。

## 各字段验证规则

### CreateUserRequest / UpdateUserRequest

| 字段 | 规则 | 错误消息 |
|------|------|----------|
| nickname | 必填 | 用户昵称不能为空 |
| nickname | 最长 50 字符 | 用户昵称长度不能超过50个字符 |
| realName | 必填 | 真实姓名不能为空 |
| realName | 最长 50 字符 | 真实姓名长度不能超过50个字符 |
| gender | 范围 0-2 | 性别值必须在0-2之间 |
| birthDate | 不能晚于当天（FluentValidation） | 出生日期不能晚于当前日期 |

### BatchDeleteRequest

| 字段 | 规则 | 错误消息 |
|------|------|----------|
| ids | 必填 | 删除ID列表不能为空 |
| ids | 至少 1 项 | 删除ID列表不能为空 |
