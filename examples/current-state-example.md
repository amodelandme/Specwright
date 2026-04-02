# Current State — FeatureFlagService

---

## Table of Contents

- [Status Summary](#-status-summary)
- [What Is Completed](#-what-is-completed)
- [What Is Not Yet Built](#-what-is-not-yet-built-phase-1-remaining)
- [Known Issues](#-known-issues)
- [Current Focus](#-current-focus)
- [What Not To Do Right Now](#-what-not-to-do-right-now)
- [Definition of Done — Phase 1](#-definition-of-done--phase-1)
- [Notes for AI Assistants](#-notes-for-ai-assistants)

---

## 📍 Status Summary

**Phase 0 — Foundation: ✅ Complete**
**Phase 1 — Architectural Cleanup: ✅ Complete**
**Phase 1 — Validation & Sanitization: ✅ Complete**
**Phase 1 — CI/CD Foundation (PRs #33, #34): ✅ Complete**
**Phase 1 — CI/CD AI Reviewer (PR #35): ✅ Complete**
**Phase 1 — Error Handling (PR #36): ✅ Complete**
**Phase 1 — Testing & Developer Experience: 🔄 In Progress**

FluentValidation v12 is implemented with manual controller validation. `InputSanitizer`
is in place. The service layer sanitizes evaluation inputs. KI-003 is closed.

GitHub Actions CI pipeline is live with parallel `lint-format` and `build-test` jobs.
CSharpier is the formatting source of truth. AI reviewer job is live — activated by
the `ai-review` label on any PR.

Global exception middleware is in place. All controllers contain only the happy path.
All error responses return a consistent `ProblemDetails` shape with
`Content-Type: application/problem+json`. AI reviewer system prompt updated to enforce
the no-try/catch-in-controllers rule.

**Product direction locked:** Azure-native, .NET-first, AI-assisted feature flag
platform targeting .NET teams on Azure. Phase 1.5 introduces Key Vault, Application
Insights, and the AI analysis endpoint immediately after Phase 1 completes.

---

## ✅ What Is Completed

### Domain Layer

- `Flag` entity with controlled mutation (private setters, explicit update methods)
- `Flag.Update()` — atomic method that sets enabled state, strategy, and `UpdatedAt`
- `FeatureEvaluationContext` value object — `IEquatable<T>`, guard clauses,
  immutable `IReadOnlyList<string>` roles, accepts `IEnumerable<string>` on construction
- `RolloutStrategy` enum (None, Percentage, RoleBased)
- `EnvironmentType` enum (None = 0 sentinel, Development, Staging, Production)
- `IRolloutStrategy` interface — `StrategyType` property for registry dispatch
- `IFeatureFlagRepository` interface — async, `CancellationToken` throughout

### Application Layer

- `NoneStrategy`, `PercentageStrategy`, `RoleStrategy` — all strategies implemented
- `FeatureEvaluator` — registry dispatch pattern
- `FeatureFlagService` — async, orchestrates repository + evaluator; sanitizes inputs
  at two call sites (`IsEnabledAsync`, `CreateFlagAsync`)
- DTOs: `CreateFlagRequest`, `UpdateFlagRequest`, `FlagResponse`, `EvaluationRequest`,
  `FlagMappings`
- `IFeatureFlagService` — async, CancellationToken, full CRUD + evaluation

### Infrastructure & API Layer

- `FeatureFlagRepository` — full EF Core implementation with Postgres + `jsonb`
- `FeatureFlagsController` — full CRUD (GET all, GET by name, POST, PUT, DELETE soft-archive)
- `EvaluationController` — POST `/api/evaluate`
- OpenAPI enrichment — Scalar UI, `EvaluationResponse` DTO, XML docs,
  `EnumSchemaTransformer`, `ApiInfoTransformer`
- `GenerateDocumentationFile=true` + `<NoWarn>1591</NoWarn>` in `Directory.Build.props`

### Validation & Sanitization (PR #30) ✅

- `InputSanitizer` — `internal static` helper in `FeatureFlag.Application/Validators/`;
  trims whitespace and strips ASCII control characters; called by validators and service layer
- `CreateFlagRequestValidator`, `UpdateFlagRequestValidator`, `EvaluationRequestValidator`
  — FluentValidation v12, `.Must()` pattern, no `.Transform()`
- Manual `ValidateAsync` in controllers — no `FluentValidation.AspNetCore`

### CI/CD — Core Pipeline (PRs #33, #34) ✅

- `.editorconfig`, `.gitattributes`, `.csharpierrc.json`, `.csharpierignore`
- `.github/workflows/ci.yml` — `lint-format` and `build-test` parallel jobs
- `dotnet csharpier check .` — format gate
- `dotnet build -p:TreatWarningsAsErrors=true` — zero-warnings policy
- `dotnet test --filter "Category!=Integration"` — unit tests only in Phase 1
- All existing tests decorated `[Trait("Category", "Unit")]`

### CI/CD — AI Reviewer (PR #35) ✅

- `.github/workflows/ci.yml` — `ai-review` job fully implemented
- `.github/prompts/ai-review-system.md` — system prompt in repo, read at runtime
- Activated by `ai-review` label on any PR
- Fail-open — transient API failures do not block merge
- `ANTHROPIC_API_KEY` secret added to GitHub repo
- `ai-review` label created in GitHub repo

### Error Handling (PR #36) ✅

- `FeatureFlagException` — abstract base class in `Domain/Exceptions/`, carries `StatusCode`
- `FlagNotFoundException` — 404, `sealed`, thrown by service layer on null flag lookup
- `DuplicateFlagNameException` — 409, `sealed`, defined but not yet thrown
  (name uniqueness check is a separate task)
- `GlobalExceptionMiddleware` — single catch-all in `Api/Middleware/`;
  domain exceptions → named 4xx; unexpected exceptions → `LogError` + safe 500
- `FrameworkReference` to `Microsoft.AspNetCore.App` added to `FeatureFlag.Domain.csproj`
- Middleware registered first in `Program.cs` — wraps entire pipeline
- `FeatureFlagService` throws `FlagNotFoundException` — no `KeyNotFoundException`
  references remain in Application layer
- `FeatureFlagsController` and `EvaluationController` contain zero `try/catch` blocks
- AI reviewer system prompt Rule 8 updated — `try/catch` in a controller is a
  reviewable error

### Dev Environment

- DevContainer: `devcontainers/base:ubuntu-24.04` + .NET 10 SDK
- Docker-outside-of-Docker configured
- `dotnet-ef` and `csharpier` in `.config/dotnet-tools.json`
- Connection string: `Host=postgres`
- `docker-compose.yml` at repo root
- All five `.csproj` files targeting `net10.0`

### Tests

- `FeatureEvaluationContextTests` — 8/8 passing, decorated `[Trait("Category", "Unit")]`
- Build: ✅ 0 warnings, 0 errors
- CSharpier: ✅ 41 files checked, 0 violations

---

## ❌ What Is Not Yet Built (Phase 1 Remaining)

### Validation (remaining)
- Name uniqueness check at the service layer before hitting the DB

### Error Handling (remaining)
- Route parameter guard for `{name}` on GET and PUT — closes KI-008

### Testing
- Unit tests for `PercentageStrategy`, `RoleStrategy`, `NoneStrategy`
- Unit tests for `FeatureEvaluator` — dispatch, missing strategy fallback
- Unit tests for all three validators — every acceptance criterion covered
- Integration tests for all API endpoints including `/api/evaluate`

### Developer Experience
- `.http` smoke test request file committed to repo (`requests/smoke-test.http`)
- Seed data for development/staging flags
- Evaluation decision logging

---

## ⚠️ Known Issues

### KI-002 — `FeatureEvaluator.Evaluate` Has an Implicit Precondition

**Severity:** Low
**Status:** Documented — tracked for review when new callers are introduced

Callers must check `Flag.IsEnabled` before calling `Evaluate`. Documented via XML
doc comment, not enforced by a guard clause.

---

### KI-006 — `Microsoft.EntityFrameworkCore.Design` Required on Both Projects

**Severity:** Low — spec convention, not a runtime issue
**Status:** Documented

Any spec with EF Core migration steps must list this package on both Infrastructure
and Api projects with `PrivateAssets=all`.

---

### KI-007 — Devcontainer Networking Requires Postgres to Start First

**Severity:** Low — inconvenience, not a bug
**Status:** Mitigated — `postStartCommand` automates the network join

**Workaround:** Run `docker compose up -d` before opening the devcontainer.
If devcontainer is already running:
```bash
docker network connect featureflagservice_default $(cat /etc/hostname)
```
**Longer-term fix:** Full docker-compose devcontainer setup. Deferred to Phase 8.

---

### KI-008 — Route Parameters on GET and PUT Lack Allowlist Validation

**Severity:** Low
**Status:** Open — Phase 1 fix

`GET /api/flags/{name}` and `PUT /api/flags/{name}` accept a `name` route parameter
with no character allowlist validation. EF Core parameterized queries prevent SQL
injection. Risk is unexpected characters reaching logs and repository calls.

**Planned fix:** Static `RouteParameterGuard.ValidateName(string name)` helper
returning `400` for non-conforming values, called at top of affected controller actions.

---

### KI-NEW-001 — `BeValidPercentageConfig` / `BeValidRoleConfig` Duplicated

**Severity:** Low — code quality, no runtime impact
**Status:** Deferred — not a Phase 1 blocker

`BeValidPercentageConfig` and `BeValidRoleConfig` are private static methods
duplicated identically in both `CreateFlagRequestValidator` and
`UpdateFlagRequestValidator`.

**Candidate fix:** Extract to `StrategyConfigRules` internal static class in
`FeatureFlag.Application/Validators/`. Both validators call the shared methods.

---

### Spec Writing — Lessons Learned

**Audit all service methods in AC-6-style tasks:** When a spec instructs updating
methods that throw or return null, explicitly list every affected method. In PR #36,
`IsEnabledAsync` was omitted from AC-6 but caught by the implementer via the DoD.

**ProblemDetails responses require `application/problem+json`:** RFC 9457 §8.1 defines
a dedicated MIME type. Do not use `MediaTypeNames.Application.Json` for `ProblemDetails`
responses. Future specs must specify `"application/problem+json"` explicitly.

---

## 🎯 Current Focus

**Phase 1 — MVP Completion (Testing & Developer Experience)**

### Immediate Next Tasks

1. Route parameter guard for `{name}` on GET/PUT — closes KI-008
2. Name uniqueness check at the service layer before hitting the DB
3. Unit tests for `PercentageStrategy`, `RoleStrategy`, `NoneStrategy`
4. Unit tests for `FeatureEvaluator` — dispatch, missing strategy fallback
5. Unit tests for all three validators
6. Integration tests for all endpoints
7. Commit `.http` smoke test file
8. Seed data for local development
9. Evaluation decision logging

---

## 🧭 What Not To Do Right Now

- No authentication or authorization yet (Phase 3)
- No caching layer yet (Phase 6)
- No advanced rollout strategies yet (Phase 5)
- No observability pipeline yet (Phase 4) — App Insights comes in Phase 1.5
- No AI analysis endpoint yet (Phase 1.5)
- No UI work
- Do not change `Host=postgres` back to `localhost` in connection string
- Do not use `FluentValidation.AspNetCore` or `AddFluentValidationAutoValidation()` —
  both are deprecated; use manual `ValidateAsync` in controllers
- Do not use `.Transform()` — removed in FluentValidation v12
- Do not run `dotnet format` without following up with `dotnet csharpier format .` —
  CSharpier is the final formatting authority
- Do not add `try/catch` blocks to controllers — `GlobalExceptionMiddleware` handles
  all exceptions; controllers contain only the happy path

---

## 📌 Definition of Done — Phase 1

- [x] `InputSanitizer` implemented and called in validators and service layer
- [x] `FluentValidation` v12 on all three request DTOs
- [x] Manual `ValidateAsync` in controllers (POST and PUT on flags; POST on evaluate)
- [x] CSharpier formatting enforced — CI blocks on violations
- [x] `.github/workflows/ci.yml` — `lint-format` and `build-test` parallel jobs live
- [x] AI reviewer job live (PR #35) — activated by `ai-review` label
- [x] `ANTHROPIC_API_KEY` secret added to GitHub repo
- [x] `ai-review` label created in GitHub repo
- [x] Global exception middleware in place
- [x] Standardized `ProblemDetails` error response shape
- [ ] Name uniqueness check at the service layer
- [ ] Route parameter guard for `{name}` on GET/PUT — closes KI-008
- [ ] Unit tests for `PercentageStrategy`, `RoleStrategy`, `NoneStrategy`
- [ ] Unit tests for `FeatureEvaluator` — dispatch, missing strategy fallback
- [ ] Unit tests for all three validators
- [ ] Integration tests for all 6 endpoints
- [ ] `.http` smoke test file committed
- [ ] Seed data for local development
- [ ] Evaluation decision logging

---

## 🧩 Notes for AI Assistants

- The system is not production-ready
- Prioritize correctness over feature expansion
- Follow Clean Architecture — dependencies point inward toward Domain
- Work within the established layer boundaries (Api → Application → Domain ← Infrastructure)
- `IFeatureFlagService` speaks entirely in DTOs — never return `Flag` from the service
- All evaluation logic must remain deterministic and isolated from persistence
- `GlobalExceptionMiddleware` is registered first in `Program.cs` — wraps entire pipeline
- Controllers contain only the happy path — zero `try/catch` blocks anywhere in `FeatureFlag.Api`
- All error responses return `ProblemDetails` with `Content-Type: application/problem+json`
- `ProblemDetails.Type` is set to `"about:blank"` — RFC 9457 recommendation for standard HTTP errors
- Custom `type` URIs pointing to FeatureFlagService documentation will be introduced in Phase 1.5
- `FlagNotFoundException` and `DuplicateFlagNameException` live in `FeatureFlag.Domain/Exceptions/`
- `DuplicateFlagNameException` is defined but not yet thrown — name uniqueness check is a separate task
- `appsettings.Development.json` is intentionally committed — local Docker defaults only
- Connection string uses `Host=postgres` — do not change to `localhost`
- Both Infrastructure and Api projects require `Microsoft.EntityFrameworkCore.Design`
  with `PrivateAssets=all`
- Do not use `FluentValidation.AspNetCore` or `.Transform()` — see FluentValidation v12 notes above
- Any spec referencing ProblemDetails must specify `application/problem+json`, not `application/json`
- Any spec with AC-6-style service method updates must explicitly list every affected method
