# CLAUDE.md

AGENTS.md 

## Project Overview

百灵鸟 (Lark) backend template — a .NET 8 WebAPI project following a layered DDD-like architecture with SqlSugar ORM and Autofac DI. The solution name has a known typo ("Tempalte" in the .sln, "Template" in projects). The `src/frontend/` directory is a placeholder and currently empty.

## Build & Run

```bash
# From src/backend/
dotnet build SH.BackEnd.Tempalte.sln
dotnet run --project SJ.BackEnd.Template.WebAPI
```

The API runs at `http://localhost:5123` by default (see `launchSettings.json`). First build must succeed before running because Autofac module registration loads DLLs from the output directory at runtime.

## Architecture

**Solution:** `src/backend/SH.BackEnd.Tempalte.sln`

8 projects with strict dependency flow:

```
WebAPI → Extensions → Repository → IRepository → Model
                    → Services    → IServices   → Common → Model
```

| Project | Role |
|---------|------|
| **WebAPI** | ASP.NET Core host, controllers, DI composition root |
| **Extensions** | Autofac module registration + SqlSugar setup |
| **IServices** | Service interfaces (IBaseServices<>, per-entity interfaces) |
| **Services** | Service implementations, delegates to IRepository |
| **IRepository** | Repository interfaces (IBaseRepository<>) + IUnitOfWorkManage |
| **Repository** | BaseRepository<> (SqlSugar CRUD), UnitOfWorkManage |
| **Model** | Entity classes with SqlSugar attributes, PageModel<> |
| **Common** | Shared utilities, DB config types, DataBaseType enum |

## Key Patterns

### Autofac Registration (Extensions/AutofacModuleRegister.cs)
- Loads `SJ.BackEnd.Template.Services.dll` and `SJ.BackEnd.Template.Repository.dll` at runtime via `Assembly.LoadFrom` — auto-registers all implementations by convention
- `BaseRepository<>` → `IBaseRepository<>` and `BaseServices<>` → `IBaseServices<>` registered as open generics
- `UnitOfWorkManage` registered as scoped (`InstancePerLifetimeScope`)
- Controllers registered via `WebAPIAutofacModule` with property injection
- `ServiceBasedControllerActivator` replaces the default to enable Autofac controller resolution

### SqlSugar Multi-Tenancy (DBS config)
- Multi-database support configured in `appsettings.json` under `"DBS"` array
- Each database gets a `ConnId` used as tenant ID; entities use `[Tenant("1")]` attribute to select their database
- Cross-database operations: `client.GetConnectionScope("2")` to switch tenants within a transaction
- `SqlSugarScope` registered as singleton (thread-safe)

### Unit of Work & Transactions
- `UnitOfWorkManage` manages nested transactions via reference counting (`_tranCount`) and a `ConcurrentStack<string>`
- Usage pattern: `_uow.BeginTran()` → operations → `_uow.CommitTran()` / `_uow.RollbackTran()`
- `BaseRepository<>` receives `IUnitOfWorkManage` via constructor injection, gets `SqlSugarScope` from it

### Entity Conventions
- Entities in `Model/Entities/` use `[SugarTable]` for table name, `[Tenant]` for multi-DB routing
- Primary keys use `[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]`
- Insert returns snowflake IDs via `ExecuteReturnSnowflakeIdAsync()`

### Base Services / Repository
- `IBaseServices<TEntity>` / `IBaseRepository<TEntity>` provide full CRUD + pagination + multi-table join queries
- `PageModel<T>` with `ConvertTo<TOut>()` for Mapster-based DTO mapping

### Primary Constructor Convention
- Classes with **≤3 constructor parameters and no extra constructor body logic** must use C# 12 primary constructor syntax.
- Primary constructor parameters are stored as field initializers when the class needs a named backing field:

```csharp
// Good — primary constructor, ≤3 params, fields initialized from params
public class TranService(IUnitOfWorkManage db, IBaseRepository<LlmConfig> configRepo) : ITranService
{
    private readonly IUnitOfWorkManage _uow = db;
    private readonly IBaseRepository<LlmConfig> _configRepo = configRepo;
    ...
}
```

- Base class constructor arguments are passed directly from the primary constructor parameter list (e.g. `BaseServices<SysUser>(repository)`).
- Exceptions: classes that assign constructor parameters to `static` members (use explicit constructor instead).

### Documentation Language
- All product documentation under `docs/product/` MUST be written in **Chinese**.
- This applies to both initial creation and subsequent sync/update PRs.
- Spec files under `specs/` should also use Chinese for user-facing descriptions and error messages, matching the codebase convention.

### WhereIF Extension Method
- `SJ.BackEnd.Template.Common.Extensions` provides `WhereIF` extension for `Expression<Func<T, bool>>`, modeled after SqlSugar's `WhereIF` semantics.
- Signature: `expr.WhereIF(bool condition, Expression<Func<T, bool>> predicate)` — appends `predicate` via AND only when `condition` is true.
- Prefer WhereIF over explicit `if` + expression reassignment:

```csharp
// Good — declarative, single expression
Expression<Func<SysUser, bool>> whereExpression = _ => true;
whereExpression = whereExpression.WhereIF(!string.IsNullOrWhiteSpace(keyword),
    u => u.RealName.Contains(keyword) || u.Nickname.Contains(keyword));

// Avoid — imperative if-block
Expression<Func<SysUser, bool>> whereExpression = _ => true;
if (!string.IsNullOrWhiteSpace(keyword))
{
    whereExpression = u => u.RealName.Contains(keyword) || u.Nickname.Contains(keyword);
}
```

## Adding a New Entity & CRUD

1. Add entity class in `Model/Entities/` with `[SugarTable]`, `[Tenant]`, and `[SugarColumn]` attributes
2. Inject `IBaseServices<YourEntity>` in controllers — no need to create custom service/repository unless you need custom logic
3. For custom service logic: create `IYourService` in `IServices/` and `YourService` in `Services/`, Autofac auto-discovers them

## NuGet Dependencies

- **SqlSugar** (5.1.4.214) — ORM with multi-DB support (MySQL, SqlServer, Sqlite, Oracle, PostgreSQL, 达梦, 人大金仓)
- **Autofac** + **Autofac.Extensions.DependencyInjection** + **Autofac.Extras.DynamicProxy** — DI & AOP
- **Mapster** (10.0.7) — object mapping
