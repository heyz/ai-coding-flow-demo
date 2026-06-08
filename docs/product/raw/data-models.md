# 数据模型

## SysUser（用户表 sys_user）

用户实体，存储于 Tenant("2")。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | long | 主键，自增 |
| realName | string | 真实姓名 |
| nickname | string | 昵称 |
| gender | int | 性别（0=未知，1=男，2=女）|
| birthDate | DateTime? | 出生年月 |
| createdTime | DateTime | 创建时间 |

## SysRole（角色表 roles）

角色实体，存储于 Tenant("2")。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | long | 主键，自增 |
| name | string | 角色名称 |
| code | string | 角色编码 |
| description | string | 描述 |
| isSystem | bool | 是否系统内置角色 |
| sortOrder | int | 排序序号 |
| createdAt | DateTime | 创建时间 |
| updatedAt | DateTime | 更新时间 |

## LlmConfig（LLM 配置表 llm_config）

LLM 配置实体，存储于 Tenant("1")。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | long | 主键，自增 |
| provider | string(50) | 提供商名称 |
| apiKey | string(200) | API 密钥 |
| chatModel | string(100) | 对话模型 |
| embeddingModel | string(100) | 嵌入模型 |
| baseUrl | string(200) | API 基础地址 |
| isDefault | bool | 是否为默认配置 |
| createdAt | DateTime? | 创建时间 |
