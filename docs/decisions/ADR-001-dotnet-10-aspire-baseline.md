# ADR-001: .NET 10 + Aspire Baseline

**Status:** Accepted  
**Date:** 2026-04-08

---

## Context

Specwright is being designed as a codebase-aware engineering operating system for AI-assisted teams. Its target architecture includes:

- API entry points
- workflow orchestration
- deterministic analysis capabilities
- bounded context assembly
- policy and governance evaluation
- execution journaling
- supporting infrastructure for persistence, health, and diagnostics

The platform is intended to begin as a modular monolith with explicit internal boundaries, while preserving a path toward future distributed evolution when scale, ownership, or operational concerns justify it.

The project also aims to stay closely aligned with the .NET ecosystem. Specwright is .NET-first by design, with initial focus on .NET backend teams and brownfield .NET codebases.

A runtime baseline is needed now so the solution structure, project boundaries, local development topology, and operational assumptions are not invented ad hoc during implementation.

---

## Decision

Specwright will use **.NET 10** as its core runtime baseline and **.NET Aspire** as its application composition and orchestration baseline.

This means:

- core services and runtime projects will target `net10.0`
- the solution will be scaffolded around:
  - `Specwright.AppHost`
  - `Specwright.ServiceDefaults`
  - `Specwright.Api`
  - `Specwright.Core`
  - `Specwright.Infrastructure`
  - `Specwright.Worker`
- Aspire will be used to:
  - compose runtime services
  - standardize local environment wiring
  - provide service discovery
  - support health checks, telemetry, and diagnostics
  - align local development with future distributed evolution

Specwright will still begin as a **modular monolith first**, not as a microservices-first system.

Aspire is being adopted as the composition and operational backbone, not as justification for premature service fragmentation.

---

## Alternatives Considered

### 1. .NET 10 without Aspire

This would reduce initial setup complexity and keep the early runtime simpler.

Rejected because:

- it would leave service composition and local runtime topology underspecified
- health, diagnostics, and environment wiring would likely become inconsistent across projects
- it would reduce alignment with the desired future runtime shape

### 2. Earlier .NET baseline

An earlier .NET version could reduce adoption risk or tooling friction.

Rejected because:

- the architecture has already been positioned around a forward-looking .NET baseline
- Specwright is intended as a modern platform project, not a compatibility-first product
- using the chosen runtime baseline early reduces later migration churn in foundational contracts and scaffolding

### 3. Generic container or script-based orchestration only

This would keep the project more runtime-agnostic.

Rejected because:

- it would push operational composition details into ad hoc scripts and setup steps
- it would weaken the first-class .NET development experience
- it would not provide the same level of integrated service composition for the local runtime

---

## Consequences

### Positive Consequences

- establishes a clear and modern runtime baseline
- gives the platform an explicit composition model early
- improves local-first development fidelity
- supports future distributed evolution without requiring immediate service extraction
- aligns with the .NET ecosystem and the project’s .NET-first positioning
- creates a predictable place for health, telemetry, and service wiring concerns

### Negative Consequences

- increases bootstrap complexity compared with a plain solution scaffold
- introduces operational concepts earlier than a minimal single-project prototype would
- requires discipline to avoid treating Aspire as an excuse to split services too soon
- may increase the learning curve for contributors unfamiliar with Aspire

### Ongoing Implications

Future specs and implementation work should assume:

- `net10.0` as the baseline target framework
- Aspire-aware application composition
- explicit boundaries between Core, API, Infrastructure, and Worker concerns
- operational concerns are part of the platform shape, not bolt-ons

---

## Review Triggers

This decision should be revisited if:

- Aspire adds more complexity than value during runtime bootstrap
- the platform remains a small modular monolith longer than expected and Aspire becomes unnecessary overhead
- ecosystem or tooling constraints make .NET 10 materially impractical for the intended contributors or users
- distributed runtime evolution is no longer a meaningful requirement

---

## Related Artifacts

- `docs/architecture.md`
- `docs/current-state.md`
- `docs/roadmap.md`
