# AI Context — <ProjectName>

> **For AI assistants and new engineers:** This is the single source of truth for
> architectural conventions, layer rules, and hard guardrails on this project.
>
> `architecture.md`, `current-state.md`, and `roadmap.md` all reference this file
> rather than duplicating its content. If you are reading one of those documents,
> read this one too before making any changes.

**Last updated:** <YYYY-MM-DD>

---

## Table of Contents

- [Architecture Rules](#architecture-rules)
- [Layer Boundaries](#layer-boundaries)
- [Conventions and Patterns](#conventions-and-patterns)
- [What Not To Do](#what-not-to-do)
- [Tech Stack Quirks](#tech-stack-quirks)
- [Testing Conventions](#testing-conventions)
- [Current Constraints](#current-constraints)
- [Before You Change Anything](#before-you-change-anything)

---

## Architecture Rules

These are non-negotiable. Deviating from any of these requires an ADR.

- **Dependency direction is inward toward Domain.** Domain references nothing outside
  itself. Application references Domain. Infrastructure references Domain.
  Api references Application. This is enforced at the project reference level —
  if a build succeeds with a backward reference, the project structure is wrong.

- **The Application layer owns use case orchestration.** No business logic in controllers.
  No business logic in Infrastructure. If a decision is made, it is made in
  Application or Domain.

- **The service interface is the DTO boundary.** Domain entities must never appear in
  any `I<Feature>Service` method signature. DTOs flow inward across the boundary.
  Entities never flow outward. DTO-to-entity mapping happens inside the service.

- **All domain mutations go through controlled methods.** No public setters on domain
  entities. Invariants are the entity's responsibility, not the caller's.

- **Infrastructure is a plugin.** All persistence and external service access is behind
  interfaces defined in Domain or Application. Infrastructure implements them.
  No `DbContext` references in Application or Domain.

---

## Layer Boundaries

```
Api             → Application (via I<Feature>Service interfaces)
Application     → Domain     (entities, value objects, domain interfaces)
Infrastructure  → Domain     (implements repository and external service interfaces)
Domain          → (nothing)
```

**What each layer owns:**

| Layer | Owns | Never owns |
|---|---|---|
| Api | HTTP concerns, request/response DTOs, input validation | Business logic, domain entities, persistence |
| Application | Use case orchestration, DTO ↔ entity mapping, sanitization | HTTP concerns, EF Core, external HTTP clients |
| Domain | Business rules, entities, value objects, domain exceptions, repository interfaces | Framework dependencies (see Tech Stack Quirks for deliberate exceptions) |
| Infrastructure | EF Core DbContext, repository implementations, external service clients | Business logic, HTTP request handling |

---

## Conventions and Patterns

### Naming

| Pattern | Example | Used for |
|---|---|---|
| `I<Entity>Repository` | `IOrderRepository` | Persistence interfaces — defined in Domain |
| `I<Feature>Service` | `IOrderService` | Application service interfaces — defined in Application |
| `<Entity>Response` | `OrderResponse` | Read DTOs returned to callers |
| `Create<Entity>Request` | `CreateOrderRequest` | Write DTOs for creation |
| `Update<Entity>Request` | `UpdateOrderRequest` | Write DTOs for updates |
| `<Feature>Exception` | `OrderNotFoundException` | Domain exceptions — carry `StatusCode` |

### Exception Handling

- Domain exceptions extend a `<ProjectName>Exception` abstract base class that carries
  an `int StatusCode` property.
- A single `GlobalExceptionMiddleware` maps domain exceptions to `ProblemDetails`
  responses using that status code.
- **Controllers contain only the happy path.** Zero `try/catch` blocks in any controller.
- All error responses use `Content-Type: application/problem+json` (RFC 9457 §8.1).
- The 500 response `detail` field must never include exception message text —
  information disclosure risk.

### Validation

- Validation runs at the HTTP boundary, inside the controller, before any service code
  executes. Invalid requests return `400` before the service is called.
- [Document your chosen validation library and registration pattern here.]
- [Document any sanitization convention — where it runs and what it covers.]

### Dependency Injection Lifetimes

| Component type | Lifetime | Rationale |
|---|---|---|
| Scoped services (stateful per-request) | Scoped | Default for application services, repositories |
| Stateless strategy/evaluator components | Singleton | Safe to share — no mutable state |
| Validators | Scoped | Injected into controllers — must match controller lifetime |

---

## What Not To Do

> This section encodes hard-won conventions. Treat each item as a constraint,
> not a suggestion.

- **Do not add business logic to controllers.** Controllers make HTTP decisions.
  Application services make business decisions.

- **Do not expose domain entities through the service interface.** Map to DTOs
  inside the service. The controller layer must never call `.ToResponse()` or
  similar mapping methods directly.

- **Do not add public setters to domain entities.** All mutations go through
  explicit domain methods. If a property needs to change, there is a method for it.

- **Do not reference `DbContext` outside of Infrastructure.** Repository interfaces
  are defined in Domain. Their EF Core implementations live in Infrastructure.
  Nothing else touches EF Core.

- **Do not add `try/catch` to controllers.** `GlobalExceptionMiddleware` handles
  all exceptions. Controllers contain only the happy path.

- **Do not use raw SQL with string concatenation.** Parameterized queries only.
  `FromSqlRaw()` with interpolated or concatenated strings is prohibited.

- **[Add project-specific items here — deprecated packages, config values that
  must not change, naming pitfalls, patterns that were tried and rejected.]**

---

## Tech Stack Quirks

> Document surprises, deprecated API choices, and non-obvious decisions here.
> Each entry should explain what, why, and what to do instead (if applicable).
> This prevents the same mistake from being made twice.

### Template entry format:

**[Package or technology] — [brief title]**
> [What is non-obvious or surprising. What the correct approach is.
> Why the incorrect approach might look tempting. When this matters.]

---

### Example entries (replace with your own):

**FluentValidation — no `.Transform()` in v12**
> `.Transform()` was removed in FluentValidation v12. Use `.Must()` lambdas for
> rules that require a transformed value for comparison. Note: `.Must()` validates
> the cleaned value but does not mutate the DTO — sanitization must be applied
> separately in the service layer if the clean value is what you intend to persist.

**[FrameworkReference on Domain]**
> `Microsoft.AspNetCore.App` is referenced by `<ProjectName>.Domain.csproj` to
> enable `StatusCodes` constants on domain exception subclasses. This is a deliberate
> tradeoff: the named constants (`Status404NotFound`, `Status409Conflict`) carry
> semantic meaning that magic numbers do not. Scope is limited to exception classes only.
> Do not introduce additional ASP.NET Core dependencies into Domain without an ADR.

**[Connection string / environment variable]**
> `[Config key]` is set to `[value]` intentionally. Do not change this without
> understanding [the dependency it satisfies]. It affects [what].

---

## Testing Conventions

- Unit tests live in `<ProjectName>.Tests/Unit/` and are decorated
  `[Trait("Category", "Unit")]`.
- Integration tests live in `<ProjectName>.Tests/Integration/` and are decorated
  `[Trait("Category", "Integration")]`.
- CI runs `dotnet test --filter "Category!=Integration"` — integration tests require
  a running [database / external service] and are a separate gate.
- [Document your test naming convention here — e.g., `MethodName_StateUnderTest_ExpectedBehavior`.]
- [Document any shared fixtures, base classes, or test helpers that exist.]
- Mock the interfaces defined in Domain (`IRepository`, `IService`), not the
  concrete implementations. This keeps tests resilient to infrastructure changes.

---

## Current Constraints

> These are known limitations that are intentionally deferred to a future phase.
> Do not work around them — work within them until the phase that addresses them.

| Constraint | Deferred To | Rule |
|---|---|---|
| No authentication or authorization | Phase [X] | Do not build features that assume caller identity |
| No caching layer | Phase [X] | Do not optimize for DB hit frequency prematurely |
| No rate limiting | Phase [X] | Depends on authentication from Phase [X] |
| [Add constraints here] | | |

---

## Before You Change Anything

1. Read [`docs/architecture.md`](architecture.md) — understand the layer structure,
   request flows, and design tradeoffs.
2. Read [`docs/current-state.md`](current-state.md) — know what phase is active,
   what is in progress, and what the current guardrails are.
3. Read the relevant `docs/decisions/<feature>/spec.md` if working on a defined feature.
4. Check the [Known Issues](current-state.md#known-issues) section for anything that
   intersects with your work.
5. Run the build and full test suite before making changes. Establish a clean baseline.
6. If your change affects a layer boundary, a domain invariant, or the security model —
   discuss it before implementing. These are architectural decisions.
