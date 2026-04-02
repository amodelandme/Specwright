# Architecture — FeatureFlagService

---

## Table of Contents

- [Product Vision](#-product-vision)
- [Overview](#-overview)
- [High-Level Architecture](#-high-level-architecture)
- [Architectural Layers](#-architectural-layers)
  - [1. API Layer](#1-api-layer-controllers)
  - [2. Validation + Sanitization Layer](#2-validation--sanitization-layer)
  - [3. Application Layer](#3-application-layer-ifeatureflagservice)
  - [4. Evaluation Engine](#4-evaluation-engine-featureevaluator)
  - [5. Strategy Layer](#5-strategy-layer-irolloutstrategy)
  - [6. Domain Layer](#6-domain-layer)
  - [7. Data Access Layer](#7-data-access-layer-repository--ef-core)
- [Security Model](#-security-model)
- [Request Flow — Evaluation](#-request-flow-evaluation-example)
- [Request Flow — Create](#-request-flow-crud-example--create)
- [Key Design Principles](#-key-design-principles)
- [Design Tradeoffs](#-design-tradeoffs)
- [Extensibility Points](#-extensibility-points)
- [Future Architecture Considerations](#-future-architecture-considerations)
- [Notes for AI Assistants](#-notes-for-ai-assistants)
- [Summary](#-summary)

---

## 🎯 Product Vision

**FeatureFlagService is being built to become an Azure-native, .NET-first, AI-assisted
feature flag platform for teams that live in the Microsoft ecosystem.**

The competitive positioning is deliberate:

- **Azure-native** — Azure Key Vault for secrets, Application Insights for observability,
  Azure Container Apps for deployment, Azure OpenAI for AI analysis. Not bolted-on Azure
  support — designed for Azure from the ground up.
- **.NET-first** — A production-quality .NET SDK is a first-class deliverable alongside
  the service itself. Teams using ASP.NET Core, Azure Functions, and the broader .NET
  ecosystem should feel at home immediately.
- **AI-assisted** — Not a dashboard with an AI chatbot stapled to it. Flag health
  analysis, stale flag detection, rollout risk reasoning, and natural language debugging
  of evaluation decisions are core product features — not add-ons.
- **Open core** — The service is open source and self-hostable. The business model is
  managed hosting, enterprise features, and support on top of the open core.

This vision shapes every architectural decision. When choosing between two valid
technical approaches, the one that better serves Azure-native deployment, SDK
ergonomics, or AI integration wins.

---

## 🧭 Overview

FeatureFlagService is a modular backend system designed to evaluate feature flags in a
deterministic, extensible, and environment-aware manner.

The system follows a **layered architecture with strong separation of concerns**, enabling:

* Testable evaluation logic
* Flexible rollout strategies
* Clear boundaries between domain, application, and infrastructure
* A documented, phase-gated security model

---

## 🏗️ High-Level Architecture

```text
[ Client ]
     ↓
[ HTTP Boundary — Validation + Sanitization ]
  Controllers call ValidateAsync() manually — invalid requests rejected as 400
  InputSanitizer strips whitespace and control characters from string inputs
  Domain and service never see bad data
     ↓
[ Controllers (API Layer) ]
  DTOs in, DTOs out — no domain knowledge
     ↓
[ Application Layer (IFeatureFlagService) ]
  Speaks entirely in DTOs — Flag entity never crosses this boundary
  Applies InputSanitizer to evaluation context before passing to evaluator
     ↓
[ Evaluation Engine (FeatureEvaluator) ]
  Pure logic — works with domain entities internally
     ↓
[ Strategy Layer (IRolloutStrategy) ]
     ↓
[ Data Access Layer (Repository / EF Core) ]
     ↓
[ Database (Postgres) ]
```

---

## 🧱 Architectural Layers

### 1. API Layer (Controllers)

**Responsibility:**

* Handle HTTP requests
* Return appropriate responses

**Key Characteristics:**

* Thin controllers — no business logic, no domain knowledge
* Delegates all work to application layer via `IFeatureFlagService`
* Receives and returns DTOs only — never touches domain entities
* Calls `ValidateAsync()` manually on mutating actions (POST, PUT) — validation
  runs at the top of each action before any service code executes
* Swagger/OpenAPI enabled at `/openapi/v1.json`

---

### 2. Validation + Sanitization Layer

**Responsibility:**

* Reject malformed, out-of-range, or structurally invalid requests at the HTTP boundary
* Sanitize string inputs before they reach application logic

**Key Characteristics:**

* `FluentValidation.AspNetCore` is **deprecated** and is not used. Validators are
  registered as `Scoped` services via explicit `AddScoped<IValidator<T>, TValidator>()`
  calls in `DependencyInjection.cs`. Controllers inject `IValidator<T>` and call
  `ValidateAsync()` manually at the top of each mutating action.
* All three request DTOs have dedicated `AbstractValidator<T>` implementations
  in `FeatureFlag.Application/Validators/`
* `InputSanitizer` is a shared static helper — trims whitespace, strips ASCII control
  characters below 0x20 (except tab) from all string inputs
* Validators call `InputSanitizer.Clean()` inside `Must()` lambdas for rules where
  sanitization changes the outcome (e.g. regex checks). Structural rules
  (`NotEmpty`, `MaximumLength`) run on the raw value — a whitespace-only or
  oversized string fails these checks regardless of sanitization.
* `FeatureFlagService` calls `InputSanitizer` directly before evaluation — ensuring
  consistent hashing and `HashSet` lookups regardless of caller whitespace behavior
* All `400` responses use `ValidationProblemDetails` (RFC 9110 compliant)

**Validators:**

| Validator | DTO | Key Rules |
|---|---|---|
| `CreateFlagRequestValidator` | `CreateFlagRequest` | Name allowlist regex, env sentinel guard, StrategyConfig cross-field rules, 2000-char limit |
| `UpdateFlagRequestValidator` | `UpdateFlagRequest` | StrategyConfig cross-field rules, 2000-char limit |
| `EvaluationRequestValidator` | `EvaluationRequest` | UserId max 256, UserRoles max 50 entries × 100 chars each, env sentinel guard |

**Input Limits (enforced at HTTP boundary):**

| Field | Limit |
|---|---|
| `Name` | 100 characters, `^[a-zA-Z0-9\-_]+$` |
| `StrategyConfig` | 2,000 characters |
| `UserId` | 256 characters |
| `UserRoles` | 50 entries, 100 characters per role |

**Why a shared sanitizer, not just validator-level sanitization:**

FluentValidation v12 removed `.Transform()`. The v12 approach runs sanitization inside
`Must()` lambdas, which validates the cleaned value but does not mutate the DTO.
Without the service-layer sanitization calls, `" Admin "` would pass validation after
being cleaned to `"Admin"` in the `Must()` check, but `RoleStrategy` would receive
`" Admin "` — causing silent `HashSet` lookup failures that deny legitimate users
access. `InputSanitizer` is the single source of truth for both surfaces.

**Sanitization scope:**

`InputSanitizer` covers the HTTP boundary only. It is not a substitute for:
- Prompt injection defense (Phase 1.5: `IPromptSanitizer`)
- Structured logging conventions (Phase 4)
- CLI or seed data sanitization (any future non-HTTP input surface must call
  `InputSanitizer` independently)

---

### 3. Application Layer (`IFeatureFlagService`)

**Responsibility:**

* Orchestrates use cases
* Coordinates between domain, evaluator, and repository
* Owns the DTO ↔ domain entity mapping boundary
* Applies `InputSanitizer` to evaluation context before passing to the evaluator

**Key Characteristics:**

* `IFeatureFlagService` interface speaks entirely in DTOs
* `Flag` entity is constructed and mapped inside `FeatureFlagService` — never exposed
  to callers
* `ToResponse()` mapping is called inside the service, not in controllers
* `IsEnabledAsync` reconstructs `FeatureEvaluationContext` with sanitized values
  before calling the evaluator — ensuring consistent SHA256 hashing and HashSet lookups
* Acts as the hard boundary between the API world and the domain world

**Boundary Rule:**

> `Flag` domain entity must never appear in any `IFeatureFlagService` method signature.
> The controller layer must never call `.ToResponse()` directly.

---

### 4. Evaluation Engine (`FeatureEvaluator`)

**Responsibility:**

* Determines whether a feature flag is enabled for a given context
* Delegates decision-making to the appropriate strategy

**Key Characteristics:**

* Pure logic — highly testable, no side effects
* No direct database access
* Registry dispatch pattern — `Dictionary<RolloutStrategy, IRolloutStrategy>`
* Accepts `FeatureEvaluationContext` (value object from domain)
* Receives sanitized context — sanitization is applied by the service layer before
  this point

**Precondition (KI-002):**

> Callers must check `Flag.IsEnabled` before calling `Evaluate`. The evaluator is a
> pure strategy dispatcher, not a policy enforcer. This contract is documented via
> XML doc comment but not enforced by a guard clause. If a second caller is introduced,
> reconsider adding the guard.

---

### 5. Strategy Layer (`IRolloutStrategy`)

**Responsibility:**

* Encapsulates rollout logic for a specific strategy type

**Implementations:**

* `NoneStrategy` — passthrough, always returns true
* `PercentageStrategy` — deterministic SHA256 hashing into 100 buckets
* `RoleStrategy` — config-driven, case-insensitive, fail-closed role matching

**Key Characteristics:**

* Follows Strategy Pattern — open for extension, closed for modification
* Each strategy is registered as a Singleton (stateless, safe to share)
* Each strategy is independently testable
* New strategies require zero changes to `FeatureEvaluator`
* All strategies are fail-closed — misconfigured or missing config returns `false`
* Receives sanitized inputs — `InputSanitizer` runs before this point

---

### 6. Domain Layer

**Core Entities:**

* `Flag` — encapsulates business rules, private setters, explicit mutation methods

**Value Objects:**

* `FeatureEvaluationContext` — immutable, `IEquatable<T>`, guard clauses on
  construction

**Enums:**

* `RolloutStrategy` (None, Percentage, RoleBased)
* `EnvironmentType` (None = 0 sentinel, Development, Staging, Production)

**Interfaces:**

* `IRolloutStrategy` — strategy contract
* `IFeatureFlagRepository` — persistence contract

**Responsibility:**

* Encapsulate business rules
* Protect invariants via private setters and explicit update methods

**Key Principle:**

> The domain should never be in an invalid state.

---

### 7. Data Access Layer (Repository + EF Core)

**Responsibility:**

* Persist and retrieve `Flag` entities

**Key Characteristics:**

* Postgres via Npgsql
* `FlagConfiguration` uses Fluent API: enums stored as strings, `StrategyConfig` as
  `jsonb`
* Partial unique index on `(Name, Environment)` filtered to `IsArchived = false` —
  archived flags are invisible to the uniqueness constraint
* Repository filters out archived flags on all read operations
* Abstracted via `IFeatureFlagRepository` — infrastructure detail hidden from domain
  and application layers
* All queries use EF Core parameterized queries — raw SQL via `FromSqlRaw()` with
  string concatenation is prohibited by architectural convention

---

## 🔐 Security Model

The security model is documented in full in `docs/decisions/adr-input-security-model.md`.
This section summarizes the key decisions.

### Threat Actors (ranked by likelihood)

1. Misconfigured or malicious SDK clients
2. Automated scanners and bots
3. Curious or probing external developers
4. Insider threats (developers with API access)

### Mitigations In Place — Phase 1

| Threat | Mitigation |
|---|---|
| Malformed input | FluentValidation manual `ValidateAsync()` — 400 before service logic runs |
| Whitespace / control characters | `InputSanitizer` — HTTP boundary + service layer |
| SQL injection | EF Core parameterized queries — `FromSqlRaw()` with concatenation prohibited |
| Mass assignment | `sealed record` DTOs — deserializer only maps declared properties |
| Oversized payloads | Length/count limits on all string and collection inputs |
| StrategyConfig injection | JSON structure validation via `Must()` + 2000-char limit |
| EnvironmentType sentinel bypass | `NotEqual(EnvironmentType.None)` on all env fields |
| Verbose error leakage | `ValidationProblemDetails` shape — no stack traces in responses |

### Consciously Deferred (Phase-Gated)

| Item | Deferred To | Rationale |
|---|---|---|
| Authentication + Authorization | Phase 3 | Depends on deployment target decided in Phase 1.5 |
| Rate limiting | Phase 3 | Meaningful rate limits require caller identity |
| Audit logging | Phase 4 | Requires identity from Phase 3 |
| Prompt injection defense (`IPromptSanitizer`) | Phase 1.5 | Only relevant when flag data is embedded in AI prompts |
| Route parameter allowlist on GET/PUT | Phase 1 (KI-008) | Minor gap — EF Core prevents SQL injection; tracked for fix |

---

## 🔄 Request Flow (Evaluation Example)

1. Client sends `POST /api/evaluate` with `EvaluationRequest` DTO
2. **`EvaluationController` calls `ValidateAsync()`** — `EvaluationRequestValidator`
   checks all fields; invalid request returns `400` before any service code runs
3. `EvaluationController` constructs `FeatureEvaluationContext` from the (still
   unsanitized) DTO and calls `IFeatureFlagService.IsEnabledAsync`
5. **`FeatureFlagService` applies `InputSanitizer`** to `UserId` and `UserRoles` in
   the context — ensures consistent hashing and HashSet lookups
6. Service retrieves `Flag` entity from repository
7. Service checks `Flag.IsEnabled` — returns false immediately if disabled
8. Service passes sanitized `Flag` + context to `FeatureEvaluator.Evaluate`
9. Evaluator looks up strategy by `Flag.StrategyType` in registry
10. Strategy evaluates and returns bool result
11. Service returns bool to controller
12. Controller returns `{ "isEnabled": true/false }` to client

---

## 🔄 Request Flow (CRUD Example — Create)

1. Client sends `POST /api/flags` with `CreateFlagRequest` DTO
2. **`FeatureFlagsController` calls `ValidateAsync()`** — `CreateFlagRequestValidator`
   checks all fields including StrategyConfig cross-field rules; invalid request
   returns `400` before any service code runs
3. `FeatureFlagsController` passes valid DTO directly to service
4. **`FeatureFlagService` applies `InputSanitizer.Clean()`** to `Name`
5. Service constructs `Flag` entity from sanitized values
6. Service calls `IFeatureFlagRepository.AddAsync` and `SaveChangesAsync`
7. Service maps `Flag` → `FlagResponse` via `FlagMappings.ToResponse()`
8. Controller returns `201 Created` with `FlagResponse` body

---

## 🧠 Key Design Principles

### Separation of Concerns

Each layer has a single responsibility and minimal knowledge of others.

---

### DTO Boundary at the Service Interface

`IFeatureFlagService` is the hard boundary between the API world and the domain world.
DTOs cross the boundary inward. Domain entities never cross the boundary outward.
This keeps controllers stable when the domain evolves, and keeps domain logic
independent of serialization concerns.

---

### Validation at the Boundary, Sanitization at Two Points

Invalid requests are rejected at the HTTP boundary — before any application code runs.
Sanitization runs at two points: once in validators (for validation consistency) and
once in the service layer (to ensure evaluation logic receives clean values). The shared
`InputSanitizer` helper enforces this as a single source of truth.

---

### Strategy Pattern for Extensibility

New rollout strategies can be added without modifying `FeatureEvaluator` or any
existing strategy. Implement `IRolloutStrategy`, register it in DI — done.

---

### Deterministic Evaluation

Feature evaluation must always return the same result for the same input.
`PercentageStrategy` uses SHA256 hashing to ensure determinism across restarts.
`InputSanitizer` ensures that `UserId` whitespace variations do not produce different
hash results — a `" user-123 "` and `"user-123"` must map to the same bucket.

---

### Fail-Closed by Default

All strategies return `false` when configuration is null, malformed, or out of range.
A misconfigured flag silently disables the feature rather than accidentally granting
access. This is a deliberate tradeoff: a user missing a feature is preferable to a
user receiving something they should not.

---

### Domain Integrity

All mutations go through controlled methods to prevent invalid state. No public setters
on domain entities. `Flag.Update()` sets all related fields atomically.

---

### Testability First

* Evaluation logic is isolated from persistence
* Strategies are independently testable (pure functions)
* Validators are independently testable — no DI required
* `IFeatureFlagRepository` is an interface — swappable in tests
* `IFeatureFlagService` is an interface — controllers are independently testable

---

## ⚖️ Design Tradeoffs

### DTO Boundary vs Convenience

**Decision:** `IFeatureFlagService` speaks entirely in DTOs — no `Flag` entity in
signatures.

**Pros:**
* Controllers have zero domain knowledge — stable API layer
* Mapping consolidated in one place — easier to reason about
* Domain can evolve without breaking the API contract

**Cons:**
* Slight overhead — mapping `Flag → FlagResponse` inside the service
* `EnvironmentType` enum still appears on the interface — acceptable for now

---

### Enum + JSON Strategy Configuration

**Decision:** `StrategyConfig` stored as `jsonb`, deserialized at evaluation time.

**Pros:**
* Flexible — each strategy defines its own config shape
* No schema migrations required when a strategy's config changes

**Cons:**
* Config validity verified at write time only via FluentValidation — semantic
  validation (is this config actually correct at runtime?) remains the strategy's
  responsibility

---

### Two-Point Sanitization (Validator + Service)

**Decision:** `InputSanitizer` called in both FluentValidation validators and
`FeatureFlagService`.

**Pros:**
* Consistent behavior — the value that is validated is the value that is used
* Strategies receive clean inputs — no silent HashSet mismatches or hash variations
* Single source of truth — one helper, called in two places

**Cons:**
* Sanitization runs twice on evaluation requests — acceptable; both calls are O(n)
  on string length with no I/O
* Any future non-HTTP input surface (CLI, seed data) must remember to call
  `InputSanitizer` independently — documented as an architectural convention

---

### Repository Pattern over Direct DbContext

**Pros:**
* Abstraction for testing — swappable in unit tests
* Decouples persistence detail from application logic

**Cons:**
* Additional complexity
* Can be overkill for small systems — accepted cost for portfolio quality

---

### Layered Architecture vs Simplicity

**Pros:**
* Scales well — each layer can evolve independently
* Easier to reason about — one layer, one job

**Cons:**
* More boilerplate
* Slower initial development — accepted cost for long-term maintainability

---

## 🔌 Extensibility Points

* Add new `IRolloutStrategy` implementations — zero changes to evaluator required
* New strategy types require a corresponding `FluentValidation` rule in
  `CreateFlagRequestValidator` and `UpdateFlagRequestValidator` before the API will
  accept configurations for them
* Introduce caching layer between service and repository (Phase 6)
* Replace repository with external service if needed
* Add event-driven evaluation tracking (Phase 4)
* Swap Postgres for another DB — only `FlagConfiguration` and `DependencyInjection`
  in Infrastructure need updating
* `IPromptSanitizer` — Phase 1.5 extensibility point for AI prompt safety

---

## 🚀 Future Architecture Considerations

### AI Analysis Endpoint + `IPromptSanitizer` (Phase 1.5)

* `IAiFlagAnalyzer` — new service interface behind `AIController`
* `POST /api/flags/analyze` — sends flag data to AI model, returns health analysis
* `IPromptSanitizer` — new interface for sanitizing string values before embedding
  in AI prompts; specifically targets newline injection, instruction override patterns,
  and role confusion attacks
* `InputSanitizer` (HTTP boundary) is a complementary first layer, not a substitute
* See `adr-input-security-model.md` DEFERRED-004 for the full prompt injection threat
  model

---

### Azure Foundation (Phase 1.5)

* Azure Key Vault — connection string and secrets out of `appsettings.json`
* Azure Application Insights — distributed tracing, request telemetry, evaluation
  custom events
* Structured logging via Application Insights custom dimensions — values logged as
  data, not interpolated strings (log injection mitigation)

---

### Caching Layer (Phase 6)

* In-memory or Redis between `FeatureFlagService` and `FeatureFlagRepository`
* Reduce DB lookups for frequently evaluated flags
* Cache invalidation on flag update/archive

---

### Authentication + Rate Limiting (Phase 3)

* JWT bearer token authentication on all flag management endpoints
* Rate limiting on `/api/evaluate` (hot path) keyed on authenticated caller identity
* Phase 3 auth is a prerequisite for Phase 4 audit logging

---

### Event-Driven Observability (Phase 4)

* Emit evaluation events from `FeatureEvaluator` or `FeatureFlagService`
* Feed into analytics pipeline via Azure Service Bus
* Enable "Why was this flag ON/OFF?" debugging endpoint
* Audit log on flag mutations — requires identity from Phase 3

---

### Docker Compose Devcontainer (Phase 8)

* Replace current `postStartCommand` network join workaround (KI-007)
* Both devcontainer and Postgres start together on a shared network
* Eliminates startup ordering dependency

---

### Multi-Service Decomposition (Optional, Long-term)

* Feature management service (CRUD)
* Evaluation service (read-heavy, cacheable)
* Metrics/observability service

---

## 🧩 Notes for AI Assistants

* Do not introduce logic into controllers
* Do not bypass `FeatureEvaluator` — all rollout logic lives in strategies
* Preserve domain encapsulation — no public setters on `Flag`
* Prefer adding new strategies over modifying existing ones
* `IFeatureFlagService` must never expose `Flag` in any method signature
* `ToResponse()` must be called inside `FeatureFlagService` — never in controllers
* Connection string uses `Host=postgres` — do not change to `localhost`
* See KI-002 before adding new callers to `FeatureEvaluator.Evaluate`
* KI-003 is closed — `StrategyConfig` is now validated at write time via FluentValidation
* Any new `IRolloutStrategy` implementation requires a corresponding validator rule in
  `CreateFlagRequestValidator` and `UpdateFlagRequestValidator` — do not accept a new
  strategy type without adding its config validation
* Do not inline sanitization logic — always call `InputSanitizer.Clean()` or
  `CleanCollection()` from `FeatureFlag.Application.Validators`
* Any non-HTTP input surface (CLI, seeds, test helpers) must call `InputSanitizer`
  independently — auto-validation does not run outside the HTTP pipeline
* Do not sanitize `StrategyConfig` content — it is JSON, stored verbatim; only its
  length and structure are validated
* See `adr-input-security-model.md` before making changes that affect the security
  boundary, deferred mitigations, or the prompt injection threat model

---

## 📌 Summary

This system is designed to behave like a **miniature production-grade platform**, not
just a CRUD API.

Its strength lies in:

* Clear layer boundaries — each layer speaks its own language (DTOs at API, entities
  at domain)
* Deterministic logic — same input always produces same output; `InputSanitizer`
  ensures whitespace variations cannot cause evaluation inconsistencies
* A documented security model — threats are ranked, mitigations are explicit, and
  deferred risks are phase-gated with clear rationale
* Extensibility through composition — new strategies require zero changes to existing
  code, but must register their config validation rules
 
The architecture prioritizes long-term maintainability over short-term simplicity.