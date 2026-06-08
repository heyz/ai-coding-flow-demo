# Product Spec: 用户 DTO 与昵称唯一性校验

## 问题描述

当前 SysUserController 的 Create 和 Update 接口直接使用 `SysUser` 实体作为请求参数，
暴露了数据库实体结构，存在以下问题：
- 客户端可以设置 `Id`、`CreatedTime` 等应由服务端控制的字段
- 接口契约与数据库模型耦合，实体变更会直接影响 API 契约
- 缺少昵称唯一性校验，可能产生重复昵称

## 目标

1. 新增/修改用户时，校验昵称是否已存在，存在则返回明确错误提示
2. 创建 DTO 对象将 API 契约与数据库实体解耦

## 非目标

- 不修改查询接口（GetList、GetById）的返回类型（仍使用 `SysUser` 实体作为响应）
- 不添加全局模型验证中间件
- 不修改删除接口的参数类型
- 不添加其他字段的唯一性校验（仅昵称）

## 用户故事

### 故事 1: 创建用户

作为 API 调用者，当我创建用户时：
- 我发送 `POST /SysUser`，请求体包含 `Nickname`（必填）、`RealName`（必填）、
  `Gender`（选填，默认0）、`BirthDate`（选填）
- 如果昵称已存在，我收到 `{ success: false, msg: "用户昵称已存在" }`
- 如果创建成功，我收到 `{ success: true, msg: "创建成功", response: <新用户ID> }`，
  同时返回完整的新建用户信息

### 故事 2: 修改用户

作为 API 调用者，当我修改用户时：
- 我发送 `PUT /SysUser/{id}`，请求体包含 `Nickname`（必填）、`RealName`（必填）、
  `Gender`、`BirthDate`
- 如果修改的昵称已被**其他用户**占用，我收到 `{ success: false, msg: "用户昵称已存在" }`
- 如果昵称与当前用户相同（未改动昵称），应允许更新通过
- 如果修改成功，我收到 `{ success: true, msg: "更新成功", response: true }`

## 边界条件与约束

| 条件 | 行为 |
|------|------|
| 创建时昵称为空字符串 | 允许（不校验空昵称的唯一性） |
| 修改时昵称为空字符串 | 允许（不校验空昵称的唯一性） |
| 修改昵称为已存在用户的昵称 | 返回 "用户昵称已存在" |
| 修改昵称与原昵称相同 | 允许通过，不视为冲突 |
| 修改不存在的用户 | 返回失败 |

## DTO 定义

### CreateUserRequest

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Nickname | string | 是 | 用户昵称 |
| RealName | string | 是 | 真实姓名 |
| Gender | int | 否 | 性别 (1-男, 2-女, 0-未知)，默认 0 |
| BirthDate | DateTime? | 否 | 出生年月 |

### UpdateUserRequest

与 `CreateUserRequest` 结构相同（字段一致），独立类型以便未来差异化扩展。

### CreateUserResponse

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | long | 新创建的用户 ID |
| Nickname | string | 昵称 |
| RealName | string | 真实姓名 |
| Gender | int | 性别 |
| BirthDate | DateTime? | 出生年月 |
| CreatedTime | DateTime | 创建时间 |

## 接受标准

- [ ] `POST /SysUser` 接受 `CreateUserRequest`，返回 `ApiResponse<CreateUserResponse>`
- [ ] `PUT /SysUser/{id}` 接受 `UpdateUserRequest`，返回 `ApiResponse<bool>`
- [ ] 创建时昵称重复返回 `success: false`，msg 为 "用户昵称已存在"
- [ ] 修改时昵称重复（被其他用户占用）返回 `success: false`，msg 为 "用户昵称已存在"
- [ ] 修改昵称与原昵称相同时允许通过
- [ ] `GET /SysUser/list` 和 `GET /SysUser/{id}` 保持现有行为不变
- [ ] `DELETE /SysUser/{id}` 保持现有行为不变
- [ ] `dotnet build` 零错误通过

## 验证方式

- `dotnet build` 编译通过
- 手动或集成测试验证昵称重复场景（正常创建、重复昵称创建、修改昵称冲突、修改昵称不变）
