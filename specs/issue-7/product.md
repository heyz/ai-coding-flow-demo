# Product Spec: 全局模型验证中间件

**Issue:** #7
**Status:** Draft

Figma: none provided

---

## 1. Summary

为所有 API 接口添加全局模型验证机制，自动拦截参数不合法的请求并返回统一格式的验证错误响应。开发者只需在 DTO 上声明验证规则（Data Annotations 或 FluentValidation），无需在 Controller 中手动检查 `ModelState.IsValid`。

## 2. Problem

当前代码库中：

- 所有 DTO 都是裸 POCO，没有任何验证属性（无 `[Required]`、`[StringLength]` 等）
- Controller 中从不检查 `ModelState.IsValid`，无效请求直接穿透到 Service 层甚至数据库
- `[ApiController]` 自带的自动 400 响应返回的是 `ValidationProblemDetails` 格式，与项目统一的 `ApiResponse<T>` 格式不一致
- 没有集中的验证错误处理，每个接口的验证逻辑需要各自实现

这导致：前端无法得到一致的错误格式，后端开发者需要重复编写验证代码，且容易遗漏验证。

## 3. Goals

1. **自动拦截**：请求参数不合法时，在进入 Controller Action 之前自动返回验证错误，Action 方法不会被执行
2. **统一响应格式**：验证失败时返回 `ApiResponse<object>` 格式，与现有成功/异常响应格式一致
3. **声明式验证**：验证规则通过 Data Annotations（`[Required]`、`[StringLength]` 等）声明在 DTO 属性上
4. **可扩展**：支持 FluentValidation 处理复杂验证逻辑，与 Data Annotations 并存
5. **零侵入**：已有的 Controller 代码无需修改，只需在 DTO 上添加验证属性即可生效
6. **详细错误信息**：验证失败时返回字段级别的错误信息，前端可以精确定位到哪个字段、什么问题

## 4. Non-goals

- 不替换现有的 `GlobalExceptionsFilter`，验证中间件与异常过滤器各司其职
- 不为现有所有 DTO 自动补全验证规则（本次只为 `CreateUserRequest` 和 `UpdateUserRequest` 添加示范性验证规则）
- 不实现前端表单联动验证或客户端验证逻辑
- 不引入自定义验证属性的开发框架（本次使用内置 Data Annotations + FluentValidation 即可）

## 5. User Experience

### 默认行为

当一个 API 请求携带了不符合验证规则的参数时：

1. 请求在中间件/过滤器层被拦截，Controller Action **不会被执行**
2. HTTP 响应状态码为 **400 Bad Request**
3. 响应体遵循 `ApiResponse<object>` 格式：

```json
{
  "status": 400,
  "success": false,
  "msg": "请求参数验证失败",
  "msgDev": "Nickname: 用户昵称不能为空; RealName: 真实姓名不能为空; Gender: 性别值必须在0-2之间",
  "response": {
    "errors": {
      "Nickname": ["用户昵称不能为空"],
      "RealName": ["真实姓名不能为空"],
      "Gender": ["性别值必须在0-2之间"]
    }
  }
}
```

### 行为规则

1. **验证时机**：在 Model Binding 之后、Action 执行之前。如果模型绑定本身就失败（例如 JSON 格式错误），也应当返回统一格式错误
2. **多字段多错误**：一个字段可以有多条验证错误（例如既非空又超长），所有错误都应收集并返回
3. **错误信息语言**：验证错误消息使用中文，与现有代码注释风格一致
4. **无验证规则的 DTO**：如果 DTO 没有任何验证属性，请求正常通过，不做任何额外处理
5. **FluentValidation 优先级**：当同一字段同时有 Data Annotation 和 FluentValidation 规则时，两者都执行，错误合并返回
6. **GET 请求的查询参数**：带有验证属性的 `[FromQuery]` 参数同样受验证中间件保护

### 边界情况

- **请求体为空**（POST/PUT 请求无 body）：应当返回验证错误，而非空引用异常
- **JSON 反序列化失败**（格式错误的 JSON）：应当返回统一格式错误，而非 500 或默认 400
- **嵌套对象验证**：DTO 中包含的对象属性如果也有验证属性，应当递归验证
- **集合验证**：DTO 中包含的集合元素如果也有验证属性，应当逐个验证

## 6. Success Criteria

1. 给 `CreateUserRequest.Nickname` 加上 `[Required]` 后，发送 `{ "Nickname": "", "RealName": "test" }` 返回 400 且包含 Nickname 字段错误信息
2. 给 `CreateUserRequest.Nickname` 加上 `[StringLength(50)]` 后，发送超过 50 字符的昵称返回 400 且包含长度错误信息
3. 给 `CreateUserRequest.Gender` 加上 `[Range(0, 2)]` 后，发送 `Gender: 5` 返回 400 且包含范围错误信息
4. 验证失败响应的 `status` 为 400，`success` 为 false，`msg` 包含人类可读的中文错误摘要
5. 验证失败响应的 `response.errors` 包含字段名到错误消息数组的映射
6. 验证通过的请求正常进入 Controller Action，行为与添加中间件之前完全一致
7. 没有验证属性的 DTO 请求正常通过，不受影响
8. 现有的 `GlobalExceptionsFilter` 仍然正常工作，与验证中间件互不干扰
9. FluentValidation 的自定义 Validator 注册后，其验证错误也被收集并合并到统一响应格式中

## 7. Validation

1. **手动测试**：使用 curl/httpie 发送包含各种无效参数的请求到 `/sysuser` 接口，验证响应格式和状态码
2. **集成测试**：对 `CreateUserRequest` 的每个验证规则编写测试用例，确认验证拦截生效且错误信息正确
3. **兼容性测试**：发送有效请求，确认仍然返回 200 和正常数据
4. **空 DTO 测试**：对没有验证属性的请求参数，确认正常通过

## 8. Open Questions

- 是否需要将验证错误信息国际化（i18n）？当前假设中文即可
- `ApiResponse<T>` 的 `msgDev` 字段中是否需要包含堆栈或更详细的技术信息？当前假设只包含字段级错误摘要
- 是否需要在验证失败时记录日志（Serilog）？当前假设不记录，因为验证失败是预期行为而非异常
