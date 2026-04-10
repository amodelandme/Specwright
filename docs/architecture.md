# Architecture - Specwright

**Last updated:** 2026-04-08

> **TL;DR**
>
> * Specwright is a .NET 10, Aspire-orchestrated engineering system for spec-driven, policy-enforced software delivery.
> * Core rule: deterministic analysis before model reasoning.
> * Source-of-truth artifacts are markdown foundation and feature documents.
> * Primary runtime stack: ASP.NET Core 10, Aspire, EF Core 10, PostgreSQL.
> * Guardrails and conventions: [`docs/ai-context.md`](ai-context.md)

---

## Table of Contents

* [Product Vision](#product-vision)
* [Overview](#overview)
* [Tech Stack](#tech-stack)
* [Project Structure](#project-structure)
* [High-Level Architecture](#high-level-architecture)
* [Core Architectural Principles](#core-architectural-principles)
* [Change Impact Model](#change-impact-model)

  * [L1 - Local Implementation Change](#l1---local-implementation-change)
  * [L2 - Feature / Workflow Change](#l2---feature--workflow-change)
  * [L3 - Architectural Change](#l3---architectural-change)
* [Team Role Mapping](#team-role-mapping)
* [Engineer Participation Model](#engineer-participation-model)
* [Workflow Integration Strategy](#workflow-integration-strategy)
* [Initial Component Boundaries](#initial-component-boundaries)

  * [1. API Layer](#1-api-layer)
  * [2. Core Layer](#2-core-layer)
  * [3. Infrastructure Layer](#3-infrastructure-layer)
  * [4. Worker Layer](#4-worker-layer)
* [Request Flow (Initial)](#request-flow-initial)
* [Current Tradeoffs](#current-tradeoffs)

  * [Modular Monolith First](#modular-monolith-first)
  * [Lexical-First Retrieval](#lexical-first-retrieval)
  * [Aspire-Orchestrated Runtime Baseline](#aspire-orchestrated-runtime-baseline)
* [Future Architecture Considerations](#future-architecture-considerations)
* [ADR Index](#adr-index)
* [Summary](#summary)

---

## Product Vision

Specwright is a codebase-aware engineering operating system for AI-assisted teams.

It provides durable project memory, bounded context assembly, deterministic analysis, and adaptive workflow governance so AI outputs remain auditable and aligned with architectural intent.

Specwright is designed to integrate with existing engineering workflows such as pull requests, CI pipelines, and repository-based documentation rather than replace them. It enables teams to apply different levels of rigor based on change impact, team structure, and delivery context.

It is designed first for .NET backend ecosystems and brownfield adoption, while preserving model-agnostic interfaces over time.

---

## Overview

Specwright is organized into three conceptual layers:

1. **Foundation Layer** (`architecture.md`, `current-state.md`, `roadmap.md`, `ai-context.md`)
2. **Spec Layer** (`spec.md`, `implementation-notes.md`)
3. **Execution Layer** (workflows, capabilities, agents, retrieval, policies, and CI gates)

The runtime model is deterministic-first: code and document facts are extracted before any model invocation. LLMs operate on bounded context packets with explicit scope, evidence, and token budgets.

---

## Tech Stack

| Concern       | Technology                              | Notes                                                        |
| ------------- | --------------------------------------- | ------------------------------------------------------------ |
| Runtime       | .NET 10 (`net10.0`)                     | LTS baseline for all core services                           |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) | Dev-time orchestration, service discovery, health, telemetry |
| API           | ASP.NET Core 10                         | Workflow entry points for CLI/UI/editor/CI integrations      |
| Data Access   | EF Core 10 + Npgsql                     | PostgreSQL persistence and metadata/indexing                 |
| Database      | PostgreSQL                              | Primary system store for workflow records and metadata       |
| Analysis      | Roslyn                                  | Deterministic .NET code and symbol analysis                  |
| Retrieval     | Metadata + lexical-first                | Semantic retrieval is optional, not default                  |
| Testing       | xUnit                                   | Unit and integration tests across capabilities/workflows     |
| CI            | GitHub Actions (planned)                | Build/test/policy checks and PR gates                        |

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
[CLI / UI / Editor / CI / PR Events]
              |
              v
      [API Entry Points]
              |
              v
   [Workflow Orchestrator]
              |
     +--------+--------+-------------------+
     |                 |                   |
     v                 v                   v
[Capability Host] [Context Assembler] [Policy & Governance Engine]
     |                 |                   |
     |                 |                   +--> role/capability policy
     |                 |                   +--> impact-level routing
     |                 |                   +--> approval paths
     |                 |                   +--> CI/PR gate decisions
     v                 v
[Deterministic Analysis] -----> [Bounded Context Packets]
                                      |
                                      v
                                 [Agent Layer]
                          Architect / Engineer / Reviewer / Analyzer
                                      |
                                      v
                              [Execution Journal]
                                      |
                                      v
                                 [PostgreSQL]
```

---

## Core Architectural Principles

1. **Deterministic-first processing**

   * Diffs, symbols, and doc facts are extracted before model reasoning.
2. **Bounded context by default**

   * Context packets define scope boundaries, evidence, unknowns, and budget.
3. **Markdown source of truth**

   * Foundation and spec artifacts remain human-readable and versioned.
4. **Capabilities over role blobs**

   * Agents are orchestrations of atomic, testable capabilities.
5. **Policy-enforced discipline**

   * Missing required artifacts or uncited claims can fail workflow outcomes.
6. **Adaptive governance**

   * Workflow strictness varies by change impact and team policy.
   * Small changes can follow lightweight paths, while architectural changes require stronger validation and approval.
   * Governance is configurable without altering core system design.
7. **Separation of proposal and approval**

   * Any role may propose changes, including architectural updates.
   * Approval authority is explicitly defined and enforced separately.
   * This preserves engineer autonomy while maintaining system integrity.
8. **Workflow integration over replacement**

   * Specwright augments existing development workflows.
   * Pull requests, CI pipelines, and repository artifacts remain first-class.
   * Specwright provides structure, traceability, and enforcement around these workflows.

---

## Change Impact Model

Specwright adapts workflow rigor based on the impact of a change.

### L1 - Local Implementation Change

* Confined to a known module or bounded area
* No architectural boundary changes
* No foundation document updates required

**Typical Flow:**
Engineer → PR → Review → Merge

---

### L2 - Feature / Workflow Change

* Touches multiple components or layers
* Requires spec creation or update
* May introduce new behaviors or flows

**Typical Flow:**
Spec → Engineer Implementation → Reviewer Validation → Merge

---

### L3 - Architectural Change

* Alters system boundaries, shared abstractions, or cross-cutting concerns
* Impacts foundation documents (`architecture.md`, `current-state.md`)
* May affect multiple teams or domains

**Typical Flow:**
Architecture Proposal → Approval → Implementation → Reviewer Validation → Foundation Update

---

This model allows teams to balance speed and rigor without enforcing a single workflow for all changes.

---

## Team Role Mapping

Specwright distinguishes between **responsibilities** and **job titles**.

* Roles such as Architect, Engineer, and Reviewer represent responsibilities within the system.
* A single individual may fulfill multiple roles, such as a Tech Lead acting as Architect.
* Role assignment is configurable and does not assume a fixed organizational structure.
* Policies are enforced based on responsibilities and capabilities, not titles.

This allows Specwright to adapt to:

* small teams with shared responsibilities
* larger teams with specialized roles
* evolving team structures over time

---

## Engineer Participation Model

Engineers are active participants in system design, not passive implementers.

Engineers are expected to:

* challenge unclear or incomplete specs
* identify inconsistencies with current-state or architecture
* propose architectural or design improvements
* document implementation realities and deviations

Engineers may:

* propose changes to foundation documents
* surface risks and tradeoffs discovered during implementation

Engineers do not:

* unilaterally approve architectural changes unless assigned that responsibility

This model ensures continuous feedback between design and implementation while preserving architectural integrity.

---

## Workflow Integration Strategy

Specwright integrates with existing engineering workflows rather than replacing them.

It is designed to work alongside:

* pull request-based development
* CI/CD pipelines
* repository-hosted markdown documentation
* issue and ticket tracking systems

Specwright adds:

* structured context for AI-assisted workflows
* policy enforcement and validation
* traceability between specs, implementation, and architecture

Pull requests remain the primary unit of code change, with Specwright providing additional validation and context around them.

---

## Initial Component Boundaries

### 1. API Layer

* Accept workflow requests from trusted entry points
* Validate request shape and invoke orchestrated workflows

### 2. Core Layer

* Workflow orchestration
* Capability contracts and implementations
* Context assembly
* Policy evaluation

### 3. Infrastructure Layer

* PostgreSQL persistence
* Indexing and retrieval storage adapters
* External model/provider adapters

### 4. Worker Layer

* Async or long-running analysis jobs
* Batch policy and review tasks

---

## Request Flow (Initial)

1. Client invokes a workflow, for example an analyzer run.
2. API validates the request and passes it to the orchestrator.
3. Deterministic capabilities collect facts from code, documents, or diffs.
4. Context assembler creates a bounded packet with citations.
5. Agent, if required, reasons over the bounded packet.
6. Policy engine validates required artifacts and evidence.
7. Execution journal records the result and supporting evidence.
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

**Cons:** Weaker semantic matching until an optional semantic layer is added.

### Aspire-Orchestrated Runtime Baseline

**Decision:** Use .NET Aspire as the application composition layer for local development and future distributed service evolution.

**Responsibilities:**

* Compose API, worker, database, and supporting services
* Provide service discovery and environment configuration
* Standardize health checks, telemetry, and diagnostics
* Enable local-first distributed development

**Pros:**

* Production-like local topology
* Clean service wiring and configuration
* Strong alignment with .NET ecosystem direction

**Cons:**

* Increased initial setup complexity

Aspire serves as the operational backbone for Specwright, enabling consistent orchestration as the system evolves from a modular monolith toward distributed components.

---

## Future Architecture Considerations

* Extract independent services from core modules only when scale or ownership boundaries justify it
* Add semantic retrieval where lexical results are insufficient for brownfield understanding
* Expand policy automation in CI for spec traceability and architecture rule compliance

---

## ADR Index

| ID      | Title                                 | Status   | Date       |
| ------- | ------------------------------------- | -------- | ---------- |
| ADR-001 | .NET 10 + Aspire baseline             | Accepted | 2026-04-08 |
| ADR-002 | PostgreSQL as primary system store    | Accepted | 2026-04-08 |
| ADR-003 | Deterministic-first architecture rule | Accepted | 2026-04-08 |

---

## Summary

* Specwright is a deterministic-first, policy-enforced engineering platform
* Foundation docs are first-class operational memory for humans and AI
* Aspire provides orchestration and observability baseline for distributed evolution
* PostgreSQL backs execution records and retrieval metadata
* Architecture favors modular capabilities and bounded context over unconstrained agent behavior
