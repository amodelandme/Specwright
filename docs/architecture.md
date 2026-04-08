# Architecture - Specwright

**Last updated:** 2026-04-08

> **TL;DR**
> - Specwright is a .NET 10, Aspire-orchestrated engineering system for spec-driven, policy-enforced software delivery.
> - Core rule: deterministic analysis before model reasoning.
> - Source-of-truth artifacts are markdown foundation and feature documents.
> - Primary runtime stack: ASP.NET Core 10, Aspire, EF Core 10, PostgreSQL.
> - Guardrails and conventions: [`docs/ai-context.md`](ai-context.md)

---

## Product Vision

Specwright is a codebase-aware engineering operating system for AI-assisted teams.

It provides durable project memory, bounded context assembly, deterministic analysis, and enforceable workflow policies so AI outputs remain auditable and aligned with architecture intent. It is designed first for .NET backend ecosystems and brownfield adoption, while preserving model-agnostic interfaces over time.

---

## Overview

Specwright is organized into three conceptual layers:

1. **Foundation Layer** (`architecture.md`, `current-state.md`, `roadmap.md`, `ai-context.md`)
2. **Spec Layer** (`spec.md`, `implementation-notes.md`)
3. **Execution Layer** (workflows, capabilities, agents, retrieval, policies, and CI gates)

The runtime model is deterministic-first: code and document facts are extracted before any model invocation. LLMs operate on bounded context packets with explicit scope, evidence, and token budgets.

---

## Tech Stack

| Concern | Technology | Notes |
|---|---|---|
| Runtime | .NET 10 (`net10.0`) | LTS baseline for all core services |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) | Dev-time orchestration, service discovery, health, telemetry |
| API | ASP.NET Core 10 | Workflow entry points for CLI/UI/editor/CI integrations |
| Data Access | EF Core 10 + Npgsql | PostgreSQL persistence and metadata/indexing |
| Database | PostgreSQL | Primary system store for workflow records and metadata |
| Analysis | Roslyn | Deterministic .NET code and symbol analysis |
| Retrieval | Metadata + lexical-first | Semantic retrieval is optional, not default |
| Testing | xUnit | Unit and integration tests across capabilities/workflows |
| CI | GitHub Actions (planned) | Build/test/policy checks and PR gates |

---

## Project Structure

```text
Specwright/
  docs/
    architecture.md
    current-state.md
    roadmap.md
    ai-context.md
    decisions/
  src/                          # to be created in bootstrap phase
    Specwright.AppHost/
    Specwright.ServiceDefaults/
    Specwright.Api/
    Specwright.Core/
    Specwright.Infrastructure/
    Specwright.Worker/
  tests/                        # to be created in bootstrap phase
    Specwright.UnitTests/
    Specwright.IntegrationTests/
  templates/
  examples/
  skills/
```

---

## High-Level Architecture

```text
[CLI / UI / Editor / CI]
          |
          v
[API Entry Points]
          |
          v
[Workflow Orchestrator]
          |
          +--> [Capability Host]
          |       - read_docs
          |       - analyze_codebase
          |       - diff_analysis
          |       - retrieve_context
          |       - validate_rules
          |
          +--> [Context Assembler]
          |
          +--> [Agent Layer]
          |       - Architect
          |       - Engineer
          |       - Reviewer
          |       - Analyzer
          |
          +--> [Policy Engine]
          |
          +--> [Execution Journal]
                  |
                  v
             [PostgreSQL]
```

---

## Core Architectural Principles

1. **Deterministic-first processing**
   - Diffs, symbols, and doc facts are extracted before model reasoning.
2. **Bounded context by default**
   - Context packets define scope boundaries, evidence, unknowns, and budget.
3. **Markdown source of truth**
   - Foundation and spec artifacts remain human-readable and versioned.
4. **Capabilities over role blobs**
   - Agents are orchestrations of atomic, testable capabilities.
5. **Policy-enforced discipline**
   - Missing required artifacts or uncited claims can fail workflow outcomes.

---

## Initial Component Boundaries

### 1. API Layer
- Accept workflow requests from trusted entry points.
- Validate request shape and invoke orchestrated workflows.

### 2. Core Layer
- Workflow orchestration
- Capability contracts and implementations
- Context assembly
- Policy evaluation

### 3. Infrastructure Layer
- PostgreSQL persistence
- Indexing and retrieval storage adapters
- External model/provider adapters

### 4. Worker Layer
- Async or long-running analysis jobs
- Batch policy and review tasks

---

## Request Flow (Initial)

1. Client invokes a workflow (for example, analyzer run).
2. API validates request and passes it to orchestrator.
3. Deterministic capabilities collect facts (code/doc/diff).
4. Context assembler creates bounded packet with citations.
5. Agent (if required) reasons over bounded packet.
6. Policy engine validates required artifacts and evidence.
7. Execution journal records result and supporting evidence.
8. API returns outcome and references.

---

## Current Tradeoffs

### Modular Monolith First
**Decision:** Start as a modular monolith with explicit internal boundaries.

**Pros:** Fast iteration, lower orchestration overhead, easier refactoring.

**Cons:** Strong discipline required to prevent boundary erosion.

### Lexical-First Retrieval
**Decision:** Metadata + lexical retrieval as default.

**Pros:** Predictable, easier to debug, strong for exact engineering facts.

**Cons:** Weaker semantic matching until optional semantic layer is added.

### Aspire for Dev Orchestration
**Decision:** Use Aspire from day one for service graph, health, telemetry, and local composition.

**Pros:** Production-like local topology, cleaner service wiring.

**Cons:** Adds early setup complexity.

---

## Future Architecture Considerations

- Extract independent services from core modules only when scale or ownership boundaries justify it.
- Add semantic retrieval where lexical results are insufficient for brownfield understanding.
- Expand policy automation in CI for spec traceability and architecture rule compliance.

---

## ADR Index

| ID | Title | Status | Date |
|---|---|---|---|
| ADR-001 | .NET 10 + Aspire baseline | Accepted | 2026-04-08 |
| ADR-002 | PostgreSQL as primary system store | Accepted | 2026-04-08 |
| ADR-003 | Deterministic-first architecture rule | Accepted | 2026-04-08 |

---

## Summary

- Specwright is a deterministic-first, policy-enforced engineering platform.
- Foundation docs are first-class operational memory for humans and AI.
- Aspire provides orchestration and observability baseline for distributed evolution.
- PostgreSQL backs execution records and retrieval metadata.
- Architecture favors modular capabilities and bounded context over unconstrained agent behavior.
