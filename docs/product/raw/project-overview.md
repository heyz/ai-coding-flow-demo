# 项目概览

百灵鸟 (Lark) 后端模板是一个基于 .NET 8 的 WebAPI 项目，采用 DDD 风格的分层架构。

## 架构分层

```
WebAPI → Extensions → Repository → IRepository → Model
                    → Services    → IServices   → Common → Model
```

## 技术栈

- **.NET 8** — ASP.NET Core WebAPI
- **SqlSugar 5.1.4** — ORM，支持多数据库
- **Autofac** — 依赖注入容器 + AOP
- **Mapster 10.0.7** — 对象映射
- **FluentValidation 11.9** — 请求参数验证
- **Xunit + Moq** — 单元测试

## 项目说明

| 项目 | 职责 |
|------|------|
| **WebAPI** | API 主机、控制器、中间件 |
| **Extensions** | Autofac 模块注册、SqlSugar 配置 |
| **Services** | 业务逻辑实现 |
| **IServices** | 服务接口定义 |
| **Repository** | 数据访问层 (SqlSugar CRUD) |
| **IRepository** | 仓储接口定义 |
| **Model** | 实体、DTO、响应模型 |
| **Common** | 公共工具类、扩展方法 |

## 编码规范

### 主构造函数
构造函数参数不超过 3 个且无额外逻辑的类，必须使用 C# 12 主构造函数语法。

### 条件表达式 WhereIF
使用 `WhereIF(condition, predicate)` 扩展方法替代 if + 表达式重新赋值模式。

### 全局模型验证
所有请求 DTO 在到达 Controller 之前由 `ValidationFilter` 自动验证，失败时返回统一 `ApiResponse<T>` 格式。

## 数据库

使用 SqlSugar 多租户配置，在 `appsettings.json` 的 `"DBS"` 节点中配置。实体通过 `[Tenant("N")]` 属性选择所属数据库。
