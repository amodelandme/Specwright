
# Global Exception Handling — Spec

**Document:** `Docs/Decisions/spec-global-exception-handling - PR#36/spec.md`
**Branch:** `feature/error-handling`
**Phase:** 1 — Error Handling
**Status:** Ready for Implementation
**PR:** #36
**Author:** Joe / Claude Architect Session
**Date:** 2026-04-01

---

## Table of Contents

- [User Story](#user-story)You’ve been designing a spec-driven development workflow powered by two complementary AI “skills”: a **Senior .NET System Architect** and a **Senior .NET Backend Engineer**. The architect’s role is to think through system design, enforce Clean Architecture, and produce structured `spec.md` documents using a standardized template. These specs don’t just outline ideas, they define implementation-ready plans, including architecture decisions, constraints, and even concrete deliverables like files and code where appropriate. You refined this role to better match your learning style by emphasizing clarity, concrete examples, and visualizations like C4 diagrams, while also making space for teaching moments that explain what’s happening under the hood in C# and ASP.NET.

Alongside that, you developed the engineering skill, which consumes those specs and follows a disciplined workflow: understand, validate, design, implement, verify, and document. The engineer reviews specs critically, challenges gaps, implements production-quality code, and generates `implementation-notes.md` to feed back into the architect’s “memory” (architecture docs, roadmap, etc.). Together, this creates a tight feedback loop between design and implementation. You also aligned everything to be model-agnostic so it works beyond Claude, and discussed organizing these skills in a dedicated repo (likely something like `dotnet-architect`) as part of a larger “developer operating system” that you can reuse, evolve, and even showcase professionally.

- [Background and Goals](#background-and-goals)
- [Design Decisions](#design-decisions)
- [Scope](#scope)
- [Exception Hierarchy](#exception-hierarchy)
  - [AC-1 FeatureFlagException — Base Class](#ac-1-featureflagexception--base-class)
  - [AC-2 FlagNotFoundException](#ac-2-flagnotfoundexception)
  - [AC-3 DuplicateFlagNameException](#ac-3-duplicateflagnamexception)
- [Middleware](#middleware)
  - [AC-4 GlobalExceptionMiddleware](#ac-4-globalexceptionmiddleware)
  - [AC-5 Middleware Registration](#ac-5-middleware-registration)
- [Service Layer Updates](#service-layer-updates)
  - [AC-6 Throw Instead of Return Null](#ac-6-throw-instead-of-return-null)
- [Controller Cleanup](#controller-cleanup)
  - [AC-7 Remove try/catch From All Controllers](#ac-7-remove-trycatch-from-all-controllers)
- [File Layout](#file-layout)
- [Implementation Notes](#implementation-notes)
- [Out of Scope](#out-of-scope)
- [Definition of Done](#definition-of-done)

---

## User Story

> As a developer integrating with FeatureFlagService, I want every error response
> to have a consistent, predictable shape — so I can handle failures reliably
> without inspecting each endpoint individually.

---

## Background and Goals

Every controller action currently wraps its logic in a `try/catch` block. This
creates three problems:

1. **Repetition.** Error handling is a cross-cutting concern — it should live in
   one place, not six.
2. **Inconsistent error shape.** Some endpoints return `NotFound()` with no body.
   Others return `NotFound(new { error = message })`. Consumers cannot rely on a
   predictable structure.
3. **Raw system exceptions leak through.** `KeyNotFoundException` is a .NET
   runtime exception with no semantic meaning to an API consumer.

This spec replaces all of the above with:

- A **domain exception hierarchy** in `FeatureFlag.Domain` that gives every
  failure mode an explicit name and HTTP status code.
- A **single `GlobalExceptionMiddleware`** in `FeatureFlag.Api` that catches
  everything, maps domain exceptions to `ProblemDetails`, logs unexpected
  errors, and returns a safe `500` for anything else.
- **Controllers with no error handling** — they contain only the happy path.

---

## Design Decisions

### Why domain exceptions in `FeatureFlag.Domain` and not `FeatureFlag.Application`?

Domain is the innermost layer. Every other layer already references it. Placing
exceptions here means `Application`, `Infrastructure`, and `Api` can all throw
and catch them with zero new project references. A `FlagNotFoundException` is a
domain concept — it represents a business fact ("this flag does not exist"), not
an infrastructure or orchestration detail.

### Why custom exception types instead of a switch on error codes?

Named exceptions are explicit and self-documenting. Adding a new failure mode
means adding a new class — the middleware never changes. This follows the
Open/Closed Principle: open for extension, closed for modification.

### Why middleware instead of `IExceptionFilter`?

An exception filter only activates if the request successfully reached the MVC
pipeline. Middleware wraps the entire HTTP pipeline — it catches errors from
routing, other middleware, and MVC alike. For a global handler, middleware is
the correct tool.

### Why `ProblemDetails` (RFC 9457)?

`ProblemDetails` is an IETF internet standard for HTTP error responses. ASP.NET
Core has first-class support for it. Consumers get a consistent, documented shape
they can parse reliably across all endpoints.

### Why `about:blank` for the `Type` field?

RFC 9457 recommends `about:blank` for standard HTTP errors with no additional
domain-specific semantics. It requires zero maintenance — no RFC section numbers
to track or get wrong. Custom URIs pointing to FeatureFlagService documentation
will be introduced in Phase 1.5 for domain-specific error types.

### Why `StatusCodes` constants instead of magic numbers?

`Microsoft.AspNetCore.Http.StatusCodes` provides named constants
(`Status404NotFound`, `Status409Conflict`) that carry both the numeric value and
its semantic meaning. Magic numbers like `404` scattered through the codebase
require the reader to know HTTP to understand the code. The Domain project adds a
`FrameworkReference` to `Microsoft.AspNetCore.App` to support this — a pragmatic
and intentional trade-off documented here.

---

## Scope

| # | What | File(s) |
|---|---|---|
| 1 | `FeatureFlagException` base class | `Domain/Exceptions/FeatureFlagException.cs` |
| 2 | `FlagNotFoundException` | `Domain/Exceptions/FlagNotFoundException.cs` |
| 3 | `DuplicateFlagNameException` | `Domain/Exceptions/DuplicateFlagNameException.cs` |
| 4 | `GlobalExceptionMiddleware` | `Api/Middleware/GlobalExceptionMiddleware.cs` |
| 5 | Middleware registration | `Api/Program.cs` |
| 6 | Service layer — throw instead of return null | `Application/Services/FeatureFlagService.cs` |
| 7 | Controller cleanup — remove all try/catch | `Api/Controllers/FeatureFlagsController.cs`, `Api/Controllers/EvaluationController.cs` |

---

## Exception Hierarchy

### AC-1: `FeatureFlagException` — Base Class

**File:** `FeatureFlag.Domain/Exceptions/FeatureFlagException.cs`

```csharp
namespace FeatureFlag.Domain.Exceptions;

/// 

/// Base class for all domain exceptions in FeatureFlagService.
/// Carries the HTTP status code that the middleware will use
/// when building the ProblemDetails response.
/// 

public abstract class FeatureFlagException : Exception
{
    /// 

    /// The HTTP status code this exception maps to.
    /// 

    public int StatusCode { get; }

    protected FeatureFlagException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
```

**Rules:**
- `abstract` — never instantiated directly, only through concrete subclasses
- `StatusCode` is read-only, set once at construction
- No ASP.NET Core references on the base class — only on the subclasses

---

### AC-2: `FlagNotFoundException`

**File:** `FeatureFlag.Domain/Exceptions/FlagNotFoundException.cs`

```csharp
using Microsoft.AspNetCore.Http;

namespace FeatureFlag.Domain.Exceptions;

/// 

/// Thrown when a feature flag cannot be found by name and environment.
/// Maps to HTTP 404 Not Found.
/// 

public sealed class FlagNotFoundException : FeatureFlagException
{
    public FlagNotFoundException(string flagName)
        : base(
            $"No feature flag with name '{flagName}' was found.",
            StatusCodes.Status404NotFound)
    {
    }
}
```

**Rules:**
- `sealed` — not intended for further subclassing
- Message format is fixed — consistent across all callers

---

### AC-3: `DuplicateFlagNameException`

**File:** `FeatureFlag.Domain/Exceptions/DuplicateFlagNameException.cs`

```csharp
using Microsoft.AspNetCore.Http;

namespace FeatureFlag.Domain.Exceptions;

/// 

/// Thrown when a flag creation attempt conflicts with an existing flag
/// of the same name in the same environment.
/// Maps to HTTP 409 Conflict.
/// 

public sealed class DuplicateFlagNameException : FeatureFlagException
{
    public DuplicateFlagNameException(string flagName)
        : base(
            $"A feature flag with name '{flagName}' already exists in this environment.",
            StatusCodes.Status409Conflict)
    {
    }
}
```

**Rules:**
- `sealed` — not intended for further subclassing
- HTTP 409 Conflict is the correct status for a uniqueness collision

> **Note for implementer:** `DuplicateFlagNameException` is defined here but not
> yet thrown. The name uniqueness check in the service layer is a separate Phase 1
> task. This exception is created now so the hierarchy is complete and the
> middleware already handles it when that task lands.

---

## Middleware

### AC-4: `GlobalExceptionMiddleware`

**File:** `FeatureFlag.Api/Middleware/GlobalExceptionMiddleware.cs`

```csharp
using FeatureFlag.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;

namespace FeatureFlag.Api.Middleware;

/// 

/// Catches all unhandled exceptions in the pipeline and returns a
/// consistent ProblemDetails response. Domain exceptions are mapped
/// to their declared HTTP status codes. All other exceptions are logged
/// and returned as a generic 500.
/// 

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FeatureFlagException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                statusCode: ex.StatusCode,
                title: GetTitleForStatusCode(ex.StatusCode),
                detail: ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetailsAsync(
                context,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: "An internal error occurred. Please try again later.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            // "about:blank" is the RFC 9457 recommendation for standard HTTP errors
            // with no additional domain-specific semantics. No maintenance required.
            // Custom URIs will be introduced in Phase 1.5 for domain-specific errors.
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                problem,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
    }

    private static string GetTitleForStatusCode(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest  => "Bad Request",
            StatusCodes.Status404NotFound    => "Not Found",
            StatusCodes.Status409Conflict    => "Conflict",
            _                                => "An error occurred"
        };
}
```

**Rules:**
- `FeatureFlagException` path: no logging — these are expected, named failures
- `Exception` path: `LogError` with full exception — operators need the stack trace
- The 500 `detail` string must never include `ex.Message` — information disclosure risk
- `ProblemDetails.Instance` must be set to `context.Request.Path`
- `Content-Type` must be `application/json` on all error responses

---

### AC-5: Middleware Registration

**File:** `FeatureFlag.Api/Program.cs`

Register `GlobalExceptionMiddleware` as the **first** middleware in the pipeline,
before all other `Use*` calls.

```csharp
// Must be first — wraps the entire pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

// All other middleware follows
app.UseHttpsRedirection();
app.MapControllers();
// etc.
```

---

## Service Layer Updates

### AC-6: Throw Instead of Return Null

**File:** `FeatureFlag.Application/Services/FeatureFlagService.cs`

Replace null returns and `KeyNotFoundException` handling with explicit
`FlagNotFoundException` throws in the following methods:

#### `GetFlagAsync`

```csharp
// Before
var flag = await _repository.GetByNameAsync(name, environment, ct);
if (flag is null) return null;
return flag.ToResponse();

// After
var flag = await _repository.GetByNameAsync(name, environment, ct);
if (flag is null)
    throw new FlagNotFoundException(name);
return flag.ToResponse();
```

#### `UpdateFlagAsync`

```csharp
// Before — try/catch KeyNotFoundException
// After — remove try/catch, throw on null lookup
var flag = await _repository.GetByNameAsync(name, environment, ct);
if (flag is null)
    throw new FlagNotFoundException(name);
```

#### `ArchiveFlagAsync`

Same pattern as `UpdateFlagAsync`.

**Rules:**
- `using FeatureFlag.Domain.Exceptions;` added to the using block
- No `try/catch` remains in `FeatureFlagService` after this change
- `KeyNotFoundException` must not be thrown or caught anywhere in the Application layer

---

## Controller Cleanup

### AC-7: Remove try/catch From All Controllers

**Files:**
- `FeatureFlag.Api/Controllers/FeatureFlagsController.cs`
- `FeatureFlag.Api/Controllers/EvaluationController.cs`

Every action method should contain only the happy path. Before and after:

```csharp
// Before
public async Task<IActionResult> GetByNameAsync(...)
{
    try
    {
        var flag = await _service.GetFlagAsync(name, environment, ct);
        return Ok(flag);
    }
    catch (KeyNotFoundException)
    {
        return NotFound();
    }
}

// After
public async Task<IActionResult> GetByNameAsync(...)
{
    var flag = await _service.GetFlagAsync(name, environment, ct);
    return Ok(flag);
}
```

Apply the same pattern to `Update`, `Archive`, and `Evaluate`.

**Rules:**
- Zero `try/catch` blocks remain in any controller after this change
- Zero `catch (KeyNotFoundException)` references remain anywhere in `FeatureFlag.Api`
- `return NotFound()` is removed from controllers
- FluentValidation `ValidateAsync` calls and their `400` returns are **not** removed —
  those are input validation, not exception handling, and belong in the controller

---

## File Layout

```
FeatureFlag.Domain/
  Exceptions/
    FeatureFlagException.cs          ← new
    FlagNotFoundException.cs         ← new
    DuplicateFlagNameException.cs    ← new

FeatureFlag.Api/
  Middleware/
    GlobalExceptionMiddleware.cs     ← new
  Program.cs                         ← modified (middleware registration)
  Controllers/
    FeatureFlagsController.cs        ← modified (remove try/catch)
    EvaluationController.cs          ← modified (remove try/catch)

FeatureFlag.Application/
  Services/
    FeatureFlagService.cs            ← modified (throw FlagNotFoundException)
```

---

## Implementation Notes

### `Microsoft.AspNetCore.Http` in Domain

`FlagNotFoundException` and `DuplicateFlagNameException` reference
`Microsoft.AspNetCore.Http.StatusCodes`. Add a framework reference to
`FeatureFlag.Domain.csproj` if not already present:

```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

No additional NuGet package is required — this is a framework reference,
not a package reference.

### Build and Format Sequence

Always run in this order:

```bash
dotnet build FeatureFlagService.sln
dotnet test --filter "Category!=Integration"
dotnet csharpier format .
dotnet csharpier check .
```

CSharpier is the final formatting authority. Run `dotnet format` first if needed,
then CSharpier.

---

## Out of Scope

- `DuplicateFlagNameException` is **defined** here but not yet **thrown** —
  the name uniqueness check is a separate task
- `RouteParameterGuard` for KI-008 — separate task
- Logging middleware for request/response tracing — Phase 4
- `OperationCanceledException` handling — ASP.NET Core handles this natively
- Structured logging configuration — Phase 1.5 (Application Insights)

---

## Definition of Done

- [ ] `FeatureFlag.Domain/Exceptions/` folder created with all three exception classes
- [ ] `FrameworkReference` added to `FeatureFlag.Domain.csproj`
- [ ] `GlobalExceptionMiddleware` created in `FeatureFlag.Api/Middleware/`
- [ ] Middleware registered first in `Program.cs`
- [ ] `FeatureFlagService` throws `FlagNotFoundException` — no `KeyNotFoundException` references remain in Application
- [ ] `FeatureFlagsController` has zero `try/catch` blocks
- [ ] `EvaluationController` has zero `try/catch` blocks
- [ ] `GET /api/flags/{name}` with unknown name returns `ProblemDetails` 404
- [ ] `PUT /api/flags/{name}` with unknown name returns `ProblemDetails` 404
- [ ] `DELETE /api/flags/{name}` with unknown name returns `ProblemDetails` 404
- [ ] `POST /api/evaluate` with unknown flag name returns `ProblemDetails` 404
- [ ] An unhandled `Exception` returns `ProblemDetails` 500 with safe detail message
- [ ] `LogError` fires for the 500 path with full exception details
- [ ] `ProblemDetails` responses include `instance` set to the request path
- [ ] `Content-Type: application/json` on all error responses
- [ ] `dotnet build FeatureFlagService.sln` → 0 errors, 0 warnings
- [ ] All existing tests passing: `dotnet test --filter "Category!=Integration"`
- [ ] CSharpier: `dotnet csharpier check .` → 0 violations
