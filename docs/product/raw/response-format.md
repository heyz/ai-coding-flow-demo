# 响应格式

所有 API 响应使用统一的 `ApiResponse<T>` 格式。

## 成功响应

```json
{
  "status": 200,
  "success": true,
  "msg": "操作成功",
  "msgDev": "",
  "response": { ... }
}
```

## 参数验证错误响应

请求参数验证失败时返回 HTTP 400：

```json
{
  "status": 400,
  "success": false,
  "msg": "用户昵称不能为空",
  "msgDev": "Nickname: 用户昵称不能为空; RealName: 真实姓名不能为空",
  "response": {
    "errors": {
      "Nickname": ["用户昵称不能为空"],
      "RealName": ["真实姓名不能为空"]
    }
  }
}
```

## 异常响应

未捕获的异常返回 HTTP 500，开发环境下 `msgDev` 包含堆栈信息：

```json
{
  "status": 500,
  "success": false,
  "msg": "异常消息",
  "msgDev": "堆栈信息（仅开发环境）",
  "response": null
}
```

## 分页响应

分页接口使用 `PageModel<T>` 作为 `response` 字段：

```json
{
  "status": 200,
  "success": true,
  "msg": "查询成功",
  "response": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "data": [ ... ]
  }
}
```
