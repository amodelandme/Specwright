# Architecture — <ProjectName>

> **TL;DR**
> - <One-sentence description of what this system does and for whom>
> - Clean Architecture: Api → Application → Domain ← Infrastructure
> - Primary stack: [e.g., ASP.NET Core 9 / EF Core 9 / PostgreSQL]
> - Security model is phase-gated — threats are ranked, mitigations are explicit
> - Guardrails and conventions: [`docs/ai-context.md`](ai-context.md)

**Last updated:** <YYYY-MM-DD>

---

## Table of Contents

- [Product Vision](#product-vision)
- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [High-Level Architecture](#high-level-architecture)
- [Architectural Layers](#architectural-layers)
  - [1. API Layer](#1-api-layer)
  - [2. Application Layer](#2-application-layer)
  - [3. Domain Layer](#3-domain-layer)
  - [4. Infrastructure Layer](#4-infrastructure-layer)
- [Security Model](#security-model)
- [Request Flows](#request-flows)
- [Key Design Principles](#key-design-principles)
- [Design Tradeoffs](#design-tradeoffs)
- [Extensibility Points](#extensibility-points)
- [Future Architecture Considerations](#future-architecture-considerations)
- [ADR Index](#adr-index)
- [Summary](#summary)

---

## Product Vision

**<ProjectName> is [a one-sentence description of the product and its deliberate
competitive or technical positioning].**

[2–4 sentences that explain the *why* behind the system's existence and design.
What ecosystem is this native to? Who is the primary user? What does it do better
or differently than existing alternatives? This section should make architectural
decisions legible — a reader should be able to trace every major design choice back
to something written here.]

**Example pillars to address:**
- What platform or ecosystem does this target? (Azure, AWS, on-prem, specific framework)
- What is the primary integration story for consumers?
- What is the open vs. proprietary split (if applicable)?
- What is the business or technical problem that existing tools do not solve well?

> This section is owned by `architecture.md`. `roadmap.md` references the vision
> stated here rather than maintaining its own version.

---

## Overview

[2–4 sentences describing the system at a structural level. What does it do at
runtime? How is it organized? What are its defining runtime characteristics — e.g.,
deterministic evaluation, event-driven, request/response, etc.?]

The system follows a **layered architecture with strict separation of concerns**,
enabling:

* [Key quality attribute — e.g., "Testable business logic fully isolated from persistence"]
* [Key quality attribute — e.g., "Flexible extension points that require zero changes to existing code"]
* [Key quality attribute — e.g., "Clear layer boundaries that make reasoning about the system straightforward"]
* [Key quality attribute — e.g., "A documented, phase-gated security model"]

---

## Tech Stack

| Concern | Technology | Notes |
|---|---|---|
| Runtime | .NET [version] | [e.g., `net9.0`, targeting LTS or current] |
| Web framework | ASP.NET Core [version] | [Controllers vs. Minimal APIs — and why] |
| ORM | EF Core [version] | [Provider: Npgsql / SQL Server / SQLite. Migration strategy.] |
| Database | [PostgreSQL / SQL Server / SQLite] | [Version. Docker vs. managed. Connection string convention.] |
| Validation | [FluentValidation vX / DataAnnotations] | [Key version notes, registration pattern] |
| Testing | [xUnit / NUnit] | [Key packages: Moq, Testcontainers, Bogus, etc.] |
| Code formatting | [CSharpier / dotnet format] | [Who is the final formatting authority and why] |
| CI | [GitHub Actions / Azure Pipelines] | [Key jobs: lint, build, test, deploy gates] |
| Containerization | Docker / docker-compose | [Local dev setup. Hosting target.] |
| Observability | [Application Insights / OpenTelemetry / Serilog] | [Phase this becomes active] |
| Secrets | [Azure Key Vault / AWS Secrets Manager / appsettings] | [Phase this becomes active] |

---

## Project Structure

```
<SolutionName>/
  <ProjectName>.Domain/              # Entities, value objects, enums, domain exceptions, interfaces
    Entities/
    ValueObjects/
    Enums/
    Exceptions/
    Interfaces/
  <ProjectName>.Application/         # Use cases, application services, DTOs, validators, mappings
    Services/
    DTOs/
    Validators/
    DependencyInjection.cs
  <ProjectName>.Infrastructure/      # EF Core, repositories, external service clients
    Persistence/
      Configurations/                # IEntityTypeConfiguration<T> classes
      Migrations/
    Repositories/
    DependencyInjection.cs
  <ProjectName>.Api/                 # Controllers, middleware, program entry
    Controllers/
    Middleware/
    Program.cs
  <ProjectName>.Tests/               # Unit and integration tests
    Unit/
    Integration/
  docker-compose.yml
  <SolutionName>.sln
  Directory.Build.props              # Shared MSBuild properties across all projects
  .editorconfig
  .gitattributes
```

---

## High-Level Architecture

```text
[ Client ]
     ↓
[ HTTP Boundary — Validation ]
  [Describe your validation approach: FluentValidation, DataAnnotations, manual, etc.]
  [Invalid requests rejected as 400 before any service code runs]
     ↓
[ API Layer (Controllers) ]
  DTOs in, DTOs out — no domain entity knowledge
     ↓
[ Application Layer (I<Feature>Service) ]
  [Describe the DTO boundary rule: entities never cross this boundary outward]
     ↓
[ Domain (Business Logic) ]
  [Describe the core processing: evaluation engine, domain rules, invariant enforcement]
     ↓
[ Infrastructure (Repository / External Services) ]
  [Repository pattern, EF Core, any external HTTP clients]
     ↓
[ Database / External Systems ]
```

---

## Architectural Layers

### 1. API Layer

**Responsibility:**
* Handle HTTP requests and return HTTP responses
* Validate incoming requests before any service code runs
* Delegate all business decisions to the Application layer

**Key Characteristics:**
* Thin controllers — no business logic, no domain entity knowledge
* Receives and returns DTOs only — domain entities never appear in controller code
* Input validation runs at the top of mutating actions before the service is called
* Zero `try/catch` blocks — `GlobalExceptionMiddleware` handles all exceptions
* [OpenAPI/Swagger details — UI location, schema customizations]

---

### 2. Application Layer

**Responsibility:**
* Orchestrate use cases
* Own the DTO ↔ domain entity mapping boundary
* Coordinate between domain logic, evaluators, and repositories

**Key Characteristics:**
* `I<Feature>Service` interface speaks entirely in DTOs — no domain entity in any method signature
* Domain entities are constructed and mapped *inside* the service — never exposed to callers
* [Document any application-layer cross-cutting concerns: sanitization, soft-delete filtering, etc.]
* Acts as the hard boundary between the API world and the domain world

**Boundary Rule:**
> `[Entity]` domain entity must never appear in any `I<Feature>Service` method signature.
> DTO-to-response mapping must be called inside the service, never in the controller.

---

### 3. Domain Layer

**Responsibility:**
* Encapsulate business rules and protect invariants
* Define the contracts for persistence and external services

**Core Entities:**
* `[Entity]` — [brief description. Note: private setters, explicit mutation methods, invariant enforcement]

**Value Objects:**
* `[ValueObject]` — immutable, `IEquatable<T>`, guard clauses enforced at construction

**Enums:**
* `[Enum]` — [brief description. Note any sentinel values and their purpose]

**Interfaces:**
* `I[Entity]Repository` — persistence contract
* `I[Feature]Service` — [if the interface is defined here rather than Application]

**Key Principle:**
> The domain should never be in an invalid state. If an entity can be constructed
> with bad data, the invariant is not being enforced correctly.

---

### 4. Infrastructure Layer

**Responsibility:**
* Persist and retrieve domain entities via EF Core
* Implement external service integrations

**Key Characteristics:**
* Abstracted behind interfaces from Domain — Infrastructure is a plugin to the system
* All EF Core entity configuration uses Fluent API via `IEntityTypeConfiguration<T>`
* [Postgres/SQL Server notes: JSON columns, partial indexes, enum storage as strings, etc.]
* Repository filters out soft-deleted records on all read operations [if applicable]
* All queries use EF Core parameterized queries — `FromSqlRaw()` with string
  concatenation is prohibited by architectural convention

---

## 🔐 Security Model

> Full decision record: [`docs/decisions/adr-security-model.md`](decisions/adr-security-model.md)

### Threat Actors (ranked by likelihood)

1. [Most likely — e.g., misconfigured or malicious API clients]
2. [Second — e.g., automated scanners and bots]
3. [Third — e.g., insider threats with direct API access]

### Mitigations In Place

| Threat | Mitigation |
|---|---|
| Malformed input | [Validation approach] — 400 before service logic runs |
| [Injection — whitespace, control chars] | [Sanitization approach] |
| SQL injection | EF Core parameterized queries — concatenated raw SQL prohibited |
| Mass assignment | Sealed record DTOs — deserializer maps only declared properties |
| Oversized payloads | [Length and count limits on string and collection fields] |
| Verbose error leakage | `ProblemDetails` shape — no stack traces in 4xx/5xx responses |

### Consciously Deferred

| Item | Deferred To | Rationale |
|---|---|---|
| Authentication + Authorization | Phase [X] | [Why: deployment target not decided, identity model unsettled, etc.] |
| Rate limiting | Phase [X] | Meaningful limits require caller identity from auth phase |
| Audit logging | Phase [X] | Requires identity |
| [Other deferred item] | Phase [X] | [Rationale] |

---

## Request Flows

### [Primary Operation] — e.g., Create / Command Flow

[Walk through the full request path from client to database and back.
Include the validation step, service boundary crossing, domain logic, and response.]

1. Client sends `[METHOD] /api/[route]` with `[RequestDTO]`
2. **Controller validates** — `[ValidatorClass]` checks all fields;
   invalid request returns `400` before any service code runs
3. Controller calls `I<Feature>Service.[MethodName]`
4. **Service [applies sanitization / constructs domain entity]**
5. Service calls `I[Entity]Repository.[Method]`
6. Service maps `[Entity]` → `[ResponseDTO]` via `[MappingMethod]`
7. Controller returns `[201 Created / 200 OK]` with response DTO

---

### [Secondary Operation] — e.g., Read / Query Flow

1. Client sends `GET /api/[route]`
2. Controller calls `I<Feature>Service.[MethodName]`
3. Service retrieves entity from repository
4. Service checks [any business rules — e.g., soft delete, authorization]
5. Service maps entity → response DTO
6. Controller returns `200 OK` with response DTO

> Add additional flows for non-trivial operations. Omit flows that are
> self-evident from the layer descriptions above.

---

## 🧠 Key Design Principles

### [Principle 1 — e.g., "DTO Boundary at the Service Interface"]

[1–3 sentences explaining the principle and why it matters for this specific system.
Make it concrete — name the interface, the entity, the layer.]

---

### [Principle 2 — e.g., "Fail-Closed by Default"]

[Same pattern. If a component can return a permissive or restrictive default on
misconfiguration, document which way it goes and why.]

---

### [Principle 3 — e.g., "Validation at the Boundary, Processing in the Domain"]

[Same pattern.]

---

### Testability as a First-Class Concern

[Describe what makes this system testable: which components are pure functions,
which interfaces are swappable in tests, what the DI configuration enables.
This should be specific — name the interfaces and components.]

---

## ⚖️ Design Tradeoffs

> Document these honestly. A reader who only sees the final decisions cannot
> judge whether the tradeoffs were considered. This section is what separates
> a thoughtful design document from a description of what was built.

### [Tradeoff 1 — e.g., "Repository Pattern over Direct DbContext"]

**Decision:** [What was decided and where it applies]

**Pros:**
* [Why this is the right call for this system]
* [What it enables — testability, flexibility, etc.]

**Cons:**
* [What it costs — boilerplate, complexity, indirection]
* [When this would be the wrong call]

---

### [Tradeoff 2 — e.g., "Layered Architecture vs. Simpler Vertical Slices"]

**Decision:** [...]

**Pros:**
* [...]

**Cons:**
* [...]

---

### [Tradeoff 3]

[Same pattern. Include at least three. More is better. Teams that skip this section
discover the tradeoffs the hard way.]

---

## 🔌 Extensibility Points

> Document the seams where the system is designed to grow without modifying
> existing code. Be specific about what "zero changes to existing code" means.

* **[Extension point 1]** — [What you add, what you don't have to change.
  e.g., "Add a new `IRolloutStrategy` implementation — zero changes to the evaluator required"]
* **[Extension point 2]** — [Same pattern]
* **[Phase X gate]** — [Extension point that is intentionally not exposed until a
  specific phase. e.g., "Caching layer between service and repository — Phase X"]
* **[Swap point]** — [Infrastructure components that can be replaced.
  e.g., "Swap PostgreSQL for another provider — only `DbContext` configuration and
  `DependencyInjection.cs` in Infrastructure need updating"]

---

## 🚀 Future Architecture Considerations

> Document structural changes that are planned but not yet built. Keep this focused
> on *architectural* changes — new layers, new services, new boundaries, new
> infrastructure. Timeline and task breakdown belongs in `roadmap.md`.

### [Phase X] — [Architectural Change Name]

* [New interface or boundary introduced]
* [New infrastructure component]
* [Impact on existing layers]

---

### [Phase Y] — [Architectural Change Name]

[Same pattern]

---

### Long-Term — [Optional Decomposition or Major Evolution]

* [If the system is a candidate for decomposition into multiple services, describe it here]
* [Note what would need to exist before that decomposition is viable]

---

## ADR Index

> Architecture Decision Records capture significant decisions, their context, and
> their rationale. They are append-only — superseded decisions are noted, not deleted.
> Store ADRs in `docs/decisions/`.

| ID | Title | Status | Date |
|---|---|---|---|
| ADR-001 | [e.g., "Use FluentValidation with manual controller invocation"] | Accepted | YYYY-MM-DD |
| ADR-002 | [e.g., "Domain exceptions carry HTTP status codes"] | Accepted | YYYY-MM-DD |
| ADR-003 | [e.g., "PostgreSQL jsonb for strategy configuration"] | Accepted | YYYY-MM-DD |

> **When to write an ADR:** Any time a decision is made that a future maintainer
> might reasonably question or want to reverse. If someone will ask "why did we
> do it this way?" — write the ADR.

---

## 📌 Summary

[3–5 bullet points that a new team member can read in 30 seconds and immediately
understand what defines this system. Focus on what makes it different from a
generic CRUD API. These should be the decisions you're most confident in and
most proud of.]

* [e.g., "Clear layer boundaries — each layer speaks its own language"]
* [e.g., "Deterministic core logic — same inputs always produce the same outputs"]
* [e.g., "Documented security model — threats ranked, mitigations explicit, deferrals phase-gated"]
* [e.g., "Extensible by composition — new [X] requires zero changes to existing [Y]"]
* [e.g., "Testability by design — [key components] are pure and independently testable"]
