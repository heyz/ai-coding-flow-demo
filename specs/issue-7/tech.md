# Tech Spec: 全局模型验证中间件

**Issue:** #7
**Status:** Draft

---

## 1. Problem

当前项目的 DTO 没有任何验证属性，Controller 不检查 `ModelState.IsValid`，无效请求直接穿透到 Service 层。`[ApiController]` 自带的自动 400 返回 `ValidationProblemDetails` 格式，与项目统一的 `ApiResponse<T>` 格式不一致。需要添加全局验证拦截机制，统一错误格式，并支持 Data Annotations 和 FluentValidation 双模式。

## 2. Relevant Code

- `src/backend/SJ.BackEnd.Template.WebAPI/Program.cs:42-44` — 当前过滤器注册点，验证过滤器应在此注册
- `src/backend/SJ.BackEnd.Template.WebAPI/Filters/GlobalExceptionsFilter.cs` — 现有异常过滤器，新验证过滤器应遵循相同的注册模式和响应格式
- `src/backend/SJ.BackEnd.Template.Model/ApiResponse.cs` — 统一响应模型，验证失败响应必须使用此格式
- `src/backend/SJ.BackEnd.Template.Model/Dtos/SysUser/CreateUserRequest.cs` — 需要添加验证属性的示范 DTO
- `src/backend/SJ.BackEnd.Template.Model/Dtos/SysUser/UpdateUserRequest.cs` — 同上
- `src/backend/SJ.BackEnd.Template.WebAPI/SJ.BackEnd.Template.WebAPI.csproj` — 需要添加 FluentValidation NuGet 引用

## 3. Current State

- **无验证**：所有 DTO 是纯 POCO，零 Data Annotation 属性，零 FluentValidation 引用
- **无拦截**：Controller 中无 `ModelState.IsValid` 检查
- **默认 400 格式不一致**：`[ApiController]` 自动返回 `ValidationProblemDetails`，与 `ApiResponse<T>` 格式不同
- **现有过滤器模式**：`GlobalExceptionsFilter` 实现 `IExceptionFilter`，通过 `o.Filters.Add(typeof(...))` 注册，返回 `ContentResult` + 手动 JSON 序列化

## 4. Proposed Changes

### 方案选择：Action Filter + SuppressModelStateInvalidFilter

**为什么选 Action Filter 而非 Middleware：**

- Action Filter 可以访问 `ActionContext.ModelState`，直接获取验证错误
- 与现有 `GlobalExceptionsFilter` 注册方式一致
- 可以通过 `SuppressModelStateInvalidFilter = true` 替换 `[ApiController]` 的默认 400 行为

**实现步骤：**

### 4.1 添加 NuGet 包

在 `SJ.BackEnd.Template.WebAPI.csproj` 中添加：

```xml
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
```

这会传递引入 `FluentValidation` 核心包。

### 4.2 创建验证失败响应模型

在 `SJ.BackEnd.Template.Model` 项目中添加 `ValidationErrorResponse.cs`：

```csharp
namespace SJ.BackEnd.Template.Model;

/// <summary>
/// 验证错误详情
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// 字段名到错误消息数组的映射
    /// </summary>
    public Dictionary<string, List<string>> errors { get; set; } = new();
}
```

放在 Model 项目是因为它属于响应 DTO 的一部分，且 `ApiResponse<T>` 也在 Model 项目中。

### 4.3 创建全局验证 Action Filter

在 `SJ.BackEnd.Template.WebAPI/Filters/` 下新建 `ValidationFilter.cs`：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SJ.BackEnd.Template.Model;

namespace SJ.BackEnd.Template.WebAPI;

/// <summary>
/// 全局模型验证过滤器
/// 替换 [ApiController] 默认的 ValidationProblemDetails 响应，
/// 统一使用 ApiResponse 格式返回验证错误
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorDict = new Dictionary<string, List<string>>();
            var errorSummaries = new List<string>();

            foreach (var (key, entry) in context.ModelState)
            {
                if (entry.Errors.Count == 0) continue;

                var fieldErrors = new List<string>();
                foreach (var error in entry.Errors)
                {
                    var msg = error.ErrorMessage;
                    if (string.IsNullOrEmpty(msg) && error.Exception != null)
                        msg = error.Exception.Message;
                    if (!string.IsNullOrEmpty(msg))
                    {
                        fieldErrors.Add(msg);
                        errorSummaries.Add($"{key}: {msg}");
                    }
                }

                if (fieldErrors.Count > 0)
                    errorDict[key] = fieldErrors;
            }

            var response = ApiResponse<ValidationErrorResponse>.Fail(
                "请求参数验证失败",
                new ValidationErrorResponse { errors = errorDict }
            );
            response.status = 400;

            // 摘要放在 msgDev
            if (errorSummaries.Count > 0)
                response.msgDev = string.Join("; ", errorSummaries);

            context.Result = new ContentResult
            {
                Content = System.Text.Json.JsonSerializer.Serialize(response),
                StatusCode = 400,
                ContentType = "application/json"
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

**设计要点：**

- `IActionFilter.OnActionExecuting` 在 Action 执行前运行，可以拦截请求
- 同时收集 Data Annotations 和 FluentValidation 产生的 ModelState 错误
- 格式与产品规格中的 JSON 结构一致
- 返回 `ContentResult` 而非 `ObjectResult`，与 `GlobalExceptionsFilter` 保持一致
- 显式设置 `StatusCode = 400` 和 `ContentType = "application/json"`

### 4.4 注册过滤器并抑制默认行为

修改 `Program.cs`：

```csharp
builder.Services.AddControllers(o => {
    o.Filters.Add(typeof(GlobalExceptionsFilter));
    o.Filters.Add(typeof(ValidationFilter));  // 新增
}).ConfigureApiBehaviorOptions(o => {
    o.SuppressModelStateInvalidFilter = true;  // 抑制 [ApiController] 默认的 400 响应
});
```

**为什么需要 `SuppressModelStateInvalidFilter`：**

`[ApiController]` 会自动注册一个 `ModelStateInvalidFilter`，它也会在 Action 执行前检查 ModelState 并返回 `ValidationProblemDetails`。如果不抑制，我们的 `ValidationFilter` 永远不会被执行（因为默认过滤器先短路了）。

### 4.5 注册 FluentValidation

在 `Program.cs` 中添加 FluentValidation 注册：

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

这会自动扫描 WebAPI 程序集中所有 `AbstractValidator<T>` 实现并注册。FluentValidation 的 `FluentValidationModelValidatorProvider` 会自动将其验证结果写入 `ModelState`，所以 `ValidationFilter` 无需直接依赖 FluentValidation API。

### 4.6 为现有 DTO 添加验证属性

**`CreateUserRequest.cs`：**

```csharp
using System.ComponentModel.DataAnnotations;

namespace SJ.BackEnd.Template.Model.Dtos.SysUser;

public class CreateUserRequest
{
    /// <summary>
    /// 用户昵称
    /// </summary>
    [Required(ErrorMessage = "用户昵称不能为空")]
    [StringLength(50, ErrorMessage = "用户昵称长度不能超过{1}个字符")]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Required(ErrorMessage = "真实姓名不能为空")]
    [StringLength(50, ErrorMessage = "真实姓名长度不能超过{1}个字符")]
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 性别 (1-男, 2-女, 0-未知)
    /// </summary>
    [Range(0, 2, ErrorMessage = "性别值必须在{1}-{2}之间")]
    public int Gender { get; set; } = 0;

    /// <summary>
    /// 出生年月
    /// </summary>
    public DateTime? BirthDate { get; set; }
}
```

**`UpdateUserRequest.cs`：** 同样添加 `[Required]`、`[StringLength]`、`[Range]`。

### 4.7 添加示范性 FluentValidation Validator

在 `SJ.BackEnd.Template.WebAPI/Validators/` 下新建 `CreateUserRequestValidator.cs`：

```csharp
using FluentValidation;
using SJ.BackEnd.Template.Model.Dtos.SysUser;

namespace SJ.BackEnd.Template.WebAPI.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        // 示范：FluentValidation 可以处理 Data Annotations 不方便表达的复杂规则
        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("出生日期不能晚于当前日期")
            .When(x => x.BirthDate.HasValue);
    }
}
```

这展示了 FluentValidation 如何补充 Data Annotations 的能力，两者错误会合并到同一个 ModelState 中。

## 5. End-to-End Flow

```
客户端发送 POST /sysuser { "Nickname": "", "RealName": "", "Gender": 5 }
    │
    ├─ Model Binding (JSON 反序列化 → CreateUserRequest)
    │   └─ Data Annotations 验证触发：Nickname Required, RealName Required, Gender Range
    │   └─ FluentValidation 触发：CreateUserRequestValidator
    │   └─ 所有错误写入 ModelState
    │
    ├─ ValidationFilter.OnActionExecuting
    │   └─ ModelState.IsValid == false
    │   └─ 收集所有字段错误 → 构建 ApiResponse<ValidationErrorResponse>
    │   └─ 设置 context.Result = ContentResult(400, JSON)
    │   └─ Action 不执行，直接返回
    │
    └─ 客户端收到 400:
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

## 6. Risks and Mitigations

| 风险 | 缓解措施 |
|------|----------|
| `SuppressModelStateInvalidFilter` 影响现有行为 | 当前没有验证属性，默认过滤器本身就没有触发过，抑制后无行为变化 |
| FluentValidation 与 Data Annotations 重复验证同一字段 | 两者错误合并到 ModelState，`ValidationFilter` 统一收集，不会丢失或重复 |
| `ContentResult` 手动序列化可能与全局 JSON 配置不一致 | 后续可改用 `ObjectResult` 配合全局 JSON 序列化配置，当前与 `GlobalExceptionsFilter` 保持一致 |
| 新增 NuGet 包增加构建时间 | FluentValidation 是轻量库，影响可忽略 |

## 7. Testing and Validation

1. **有效请求**：发送合法的 `CreateUserRequest`，确认返回 200 且数据正确
2. **空必填字段**：发送 `Nickname: ""`，确认返回 400 且错误信息正确
3. **超长字符串**：发送 51 字符的 Nickname，确认返回 400 且包含长度错误
4. **超出范围值**：发送 `Gender: 5`，确认返回 400 且包含范围错误
5. **多字段同时错误**：发送全部无效数据，确认所有字段错误都出现在 `errors` 中
6. **FluentValidation 规则**：发送未来日期的 BirthDate，确认返回 400 且包含 FluentValidation 的错误信息
7. **无验证属性的端点**：发送 GET `/sysuser/list`，确认正常返回
8. **JSON 反序列化失败**：发送格式错误的 JSON，确认返回统一格式错误（这由 `GlobalExceptionsFilter` 处理）

## 8. Follow-ups

- 考虑将 `ValidationFilter` 移入 Extensions 项目以便复用
- 考虑 `ApiResponse<T>` 添加泛型非泛型统一转换，让 `ValidationFilter` 可以使用非泛型 `ApiResponse`
- 为其他实体 DTO（如 SysRole、LlmConfig）添加验证规则
- 添加 JSON 序列化配置（camelCase、枚举转字符串等），使响应格式更规范
