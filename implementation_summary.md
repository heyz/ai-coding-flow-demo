# Implementation Summary

## Feature: 用户 DTO 与昵称唯一性校验

### What Changed

#### 新增文件

| 文件 | 说明 |
|------|------|
| `Model/Dtos/SysUser/CreateUserRequest.cs` | 创建用户请求 DTO（Nickname, RealName, Gender, BirthDate） |
| `Model/Dtos/SysUser/UpdateUserRequest.cs` | 修改用户请求 DTO（字段同 CreateUserRequest，独立类型以便未来差异化） |
| `Model/Dtos/SysUser/CreateUserResponse.cs` | 创建用户响应 DTO（返回完整用户信息含 Id, CreatedTime） |

#### 修改文件

| 文件 | 变更 |
|------|------|
| `IServices/SysUser/ISysUserService.cs` | `Create` 参数 `SysUser` → `CreateUserRequest`，返回 `long` → `CreateUserResponse?`（null=昵称重复）；`Update` 参数 `SysUser` → `UpdateUserRequest` |
| `Services/SysUser/SysUserService.cs` | 新增 `IsNicknameExists()` 私有方法；`Create` 增加昵称唯一性校验 + DTO→实体→响应映射；`Update` 增加昵称唯一性校验（排除自身）+ DTO 映射 |
| `WebAPI/Controllers/SysUserController.cs` | `Create`/`Update` 改用 DTO 参数；昵称重复时返回 `ApiResponse.Fail("用户昵称已存在")` |

### 昵称唯一性校验逻辑

- **创建**: 昵称非空 → `QueryByExpression(n => n.Nickname == nickname)` → `.Any()` → 存在则返回 null
- **修改**: 昵称非空 → `QueryByExpression(n => n.Nickname == nickname)` → `.Any(u => u.Id != excludeUserId)` → 存在则返回 false
- **空昵称**: 不校验，允许通过

### How Validated

- `dotnet build SH.BackEnd.Tempalte.sln` — 0 errors, 37 pre-existing warnings

### Specs

- `specs/user-dto-and-validation/product.md` — 产品规格
- `specs/user-dto-and-validation/tech.md` — 技术规格

### Remaining Notes

- 查询接口（GET list, GET /{id}）和删除接口（DELETE /{id}）保持原有行为不变
- 昵称校验存在并发窗口，后续可在数据库层添加唯一索引
- Update 失败时统一返回 "用户昵称已存在"，实际可能因用户不存在也返回失败 — 当前语义合并为 "更新失败"
