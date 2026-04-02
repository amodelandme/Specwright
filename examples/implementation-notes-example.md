# Global Exception Handling — Implementation Notes

**Session date:** 2026-04-01
**Branch:** `feature/error-handling`
**Spec reference:** `Docs/Decisions/spec-global-exception-handling - PR#36/spec.md`
**Build status:** Passed — 0 warnings, 0 errors
**Tests:** 8/8 passing
**PR:** #36

---

## Table of Contents

- [What Was Built](#what-was-built)
- [Spec Gaps Resolved](#spec-gaps-resolved)
- [Deviations from Spec](#deviations-from-spec)
- [File-by-File Changes](#file-by-file-changes)
- [Definition of Done — Status](#definition-of-done--status)
- [AI Reviewer System Prompt](#ai-reviewer-system-prompt)

---

## What Was Built

A domain exception hierarchy, global exception middleware, and controller cleanup
that replaces all per-controller `try/catch` blocks. Every error response now uses
a consistent `ProblemDetails` shape.

---

## Spec Gaps Resolved

### Gap 1 — `IsEnabledAsync` omitted from AC-6

AC-6 listed three methods to update (`GetFlagAsync`, `UpdateFlagAsync`,
`ArchiveFlagAsync`) but `IsEnabledAsync` also threw `KeyNotFoundException` on a
null flag lookup. The DoD item `POST /api/evaluate` returns `ProblemDetails` 404
confirmed the intent. `IsEnabledAsync` was updated to throw `FlagNotFoundException`
along with the other three methods.

### Gap 2 — Content-Type should be `application/problem+json`

The spec specified `MediaTypeNames.Application.Json` (`"application/json"`) but
the DoD claims RFC 9457 compliance. RFC 9457 §8.1 requires `application/problem+json`
for problem details responses. ASP.NET Core's own `ValidationProblem()` path also
returns `application/problem+json`. Using `"application/json"` would create
inconsistency. Confirmed with developer and updated to `"application/problem+json"`.

---

## Deviations from Spec

### `JsonSerializerOptions` cached as `static readonly`

The spec's `WriteProblemDetailsAsync` snippet created `new JsonSerializerOptions`
on every invocation. `JsonSerializerOptions` construction is expensive (builds
internal metadata caches) and is a known .NET performance anti-pattern when
allocated per-call. Changed to a `static readonly` field on the middleware class.
This is strictly correct — no behavioural change.

---

## File-by-File Changes

### New files

| File | Purpose |
|---|---|
| `FeatureFlag.Domain/Exceptions/FeatureFlagException.cs` | Abstract base — carries `StatusCode`, no ASP.NET Core reference |
| `FeatureFlag.Domain/Exceptions/FlagNotFoundException.cs` | 404 — thrown on null flag lookup |
| `FeatureFlag.Domain/Exceptions/DuplicateFlagNameException.cs` | 409 — defined, not yet thrown (name uniqueness is a separate task) |
| `FeatureFlag.Api/Middleware/GlobalExceptionMiddleware.cs` | Single catch-all handler — domain exceptions → named 4xx, everything else → safe 500 |

### Modified files

| File | Change |
|---|---|
| `FeatureFlag.Domain/FeatureFlag.Domain.csproj` | Added `<FrameworkReference Include="Microsoft.AspNetCore.App" />` for `StatusCodes` constants |
| `FeatureFlag.Api/Program.cs` | `app.UseMiddleware<GlobalExceptionMiddleware>()` registered first in pipeline |
| `FeatureFlag.Application/Services/FeatureFlagService.cs` | All four `KeyNotFoundException` throws replaced with `FlagNotFoundException`; added `using FeatureFlag.Domain.Exceptions` |
| `FeatureFlag.Api/Controllers/FeatureFlagsController.cs` | Removed `try/catch` from `GetByNameAsync`, `UpdateAsync`, `ArchiveAsync` |
| `FeatureFlag.Api/Controllers/EvaluationController.cs` | Removed `try/catch` from `EvaluateAsync`; removed `e.Message` from error response |

---

## Definition of Done — Status

- [x] `FeatureFlag.Domain/Exceptions/` folder created with all three exception classes
- [x] `FrameworkReference` added to `FeatureFlag.Domain.csproj`
- [x] `GlobalExceptionMiddleware` created in `FeatureFlag.Api/Middleware/`
- [x] Middleware registered first in `Program.cs`
- [x] `FeatureFlagService` throws `FlagNotFoundException` — no `KeyNotFoundException` references remain in Application
- [x] `FeatureFlagsController` has zero `try/catch` blocks
- [x] `EvaluationController` has zero `try/catch` blocks
- [ ] `GET /api/flags/{name}` with unknown name returns `ProblemDetails` 404 — verified by integration test (Phase 2)
- [ ] `PUT /api/flags/{name}` with unknown name returns `ProblemDetails` 404 — verified by integration test (Phase 2)
- [ ] `DELETE /api/flags/{name}` with unknown name returns `ProblemDetails` 404 — verified by integration test (Phase 2)
- [ ] `POST /api/evaluate` with unknown flag name returns `ProblemDetails` 404 — verified by integration test (Phase 2)
- [ ] An unhandled `Exception` returns `ProblemDetails` 500 with safe detail message — verified by integration test (Phase 2)
- [ ] `LogError` fires for the 500 path with full exception details — verified by integration test (Phase 2)
- [x] `ProblemDetails` responses include `instance` set to the request path
- [x] `Content-Type: application/problem+json` on all error responses
- [x] `dotnet build FeatureFlagService.sln` → 0 errors, 0 warnings
- [x] All existing tests passing: `dotnet test --filter "Category!=Integration"` → 8/8
- [x] CSharpier: `dotnet csharpier check .` → 0 violations

The four DoD items marked incomplete require a running Postgres instance and are
integration-test concerns — they will be closed in Phase 2.

---

## AI Reviewer System Prompt

Per `current-state.md` and `roadmap.md`: Rule 8 in
`.github/prompts/ai-review-system.md` currently tells the reviewer that
controllers use `try/catch` for error handling. Now that global exception
middleware is in place, Rule 8 must be updated to reflect that controllers
contain only the happy path and `GlobalExceptionMiddleware` handles all exceptions.

Rule 8 was updated in this PR. It now states that `GlobalExceptionMiddleware` is in
place, controllers must contain only the happy path, and any `try/catch` in a
controller is a reviewable error.
