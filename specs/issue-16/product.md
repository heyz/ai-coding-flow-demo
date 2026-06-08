# Product Spec: 权限模块

**Issue:** #16
**Status:** Draft

Figma: none provided

---

## 1. 概述

实现权限管理模块，支持以树形表格展示权限数据。权限分为菜单、按钮等类型，支持完整的 CRUD 操作。

## 2. 问题

当前系统缺少统一的权限管理模块。权限数据需要通过 API 进行管理，且权限具有层级结构（树形），需要支持父子关系的维护。

## 3. 目标

1. 建立权限实体（sys_permission），支持树形层级结构
2. 权限类型包括：菜单、按钮等
3. 提供完整的 CRUD 接口
4. 支持按树形结构查询权限列表

## 4. 非目标

- 不实现权限与角色的关联（后续模块）
- 不实现按钮级别的权限拦截
- 不实现前端菜单的动态生成

## 5. 用户体验

### API 接口

路由前缀：`/permission`

#### POST /permission
创建权限。

**请求体：**
```json
{
  "name": "用户管理",
  "code": "sys:user",
  "type": "menu",
  "parentId": 0,
  "path": "/system/user",
  "icon": "user",
  "sortOrder": 1
}
```

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| name | string | 是 | 权限名称 |
| code | string | 是 | 权限编码（唯一） |
| type | string | 是 | 权限类型：menu/button/api |
| parentId | long | 否 | 父级ID，0 表示根节点 |
| path | string | 否 | 前端路由路径 |
| icon | string | 否 | 图标 |
| sortOrder | int | 否 | 排序序号 |

**响应：** `ApiResponse<SysPermission>`

#### DELETE /permission/{id}
删除权限。如果有子节点则不允许删除。

**响应：** `ApiResponse<bool>`

#### PUT /permission/{id}
更新权限。

**请求体：** 同创建

**响应：** `ApiResponse<bool>`

#### GET /permission/{id}
获取单个权限详情。

**响应：** `ApiResponse<SysPermission>`

#### GET /permission/tree
获取权限树形列表。

**响应：** `ApiResponse<List<SysPermission>>` — 按 parentId 层级排序返回

### 验证规则

| 字段 | 规则 | 错误消息 |
|------|------|----------|
| name | 必填，最长 50 | 权限名称不能为空 |
| code | 必填，最长 100 | 权限编码不能为空 |
| type | 必填，menu/button/api | 权限类型无效 |

### 行为规则

1. 权限编码唯一（code 唯一索引）
2. 删除权限时检查是否存在子节点，存在则禁止删除
3. 根节点 parentId = 0

## 6. 成功标准

1. 创建权限：返回 200 包含新权限 ID
2. 创建权限（编码重复）：返回 success: false
3. 更新权限：修改后查询确认已更新
4. 删除权限（无子节点）：正常删除
5. 删除权限（有子节点）：返回 400
6. 查询树形列表：返回按层级排序的数据

## 7. 验证

1. curl 测试每个接口的合法/非法请求
2. 测试父子权限关系维护

## 8. 开放问题

- 后续需要与角色关联实现权限分配
