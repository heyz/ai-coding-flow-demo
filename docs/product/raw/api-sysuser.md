# SysUser API

用户管理模块 — 提供用户的增删改查和批量删除操作。

Base URL: `/sysuser`

## 接口列表

### GET /sysuser/list

分页查询用户列表。

**参数：**

| 名称 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | int | 否 | 1 | 页码 |
| pageSize | int | 否 | 10 | 每页条数 |
| keyword | string | 否 | null | 搜索关键词（匹配真实姓名或昵称）|

**响应：** `ApiResponse<PageModel<SysUser>>`

### GET /sysuser/{id}

根据 ID 获取用户详情。

**参数：**

| 名称 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | long | 是 | 用户 ID |

**响应：** `ApiResponse<SysUser>`

### POST /sysuser

创建用户。

**请求体：** `CreateUserRequest`

| 字段 | 类型 | 验证规则 | 说明 |
|------|------|----------|------|
| nickname | string | 必填，最长 50 字符 | 用户昵称 |
| realName | string | 必填，最长 50 字符 | 真实姓名 |
| gender | int | 范围 0-2 | 性别（0=未知，1=男，2=女）|
| birthDate | DateTime? | 可选，不能晚于当天 | 出生年月 |

**响应：** `ApiResponse<CreateUserResponse>` — 昵称重复时返回 `success: false`, `msg: "用户昵称已存在"`

### PUT /sysuser/{id}

更新用户。

**参数：**

| 名称 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | long | 是 | 用户 ID |

**请求体：** `UpdateUserRequest`（字段同 CreateUserRequest）

**响应：** `ApiResponse<bool>` — 昵称重复或用户不存在时返回 `success: false`

### DELETE /sysuser/{id}

删除单个用户。

**参数：**

| 名称 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | long | 是 | 用户 ID |

**响应：** `ApiResponse<bool>`

### DELETE /sysuser/batch

批量删除用户。

**请求体：** `BatchDeleteRequest`

| 字段 | 类型 | 验证规则 | 说明 |
|------|------|----------|------|
| ids | long[] | 必填，至少 1 项 | 要删除的用户 ID 列表 |

**响应：** `ApiResponse<int>` — `response` 字段为实际删除的用户数量，不存在的 ID 静默忽略。
