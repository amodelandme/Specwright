# Current State - Specwright

**Last updated:** 2026-04-09  
**Current phase:** Phase 0 - Foundation Stabilization (Pre-Runtime)

> This document reflects the actual, verifiable state of the system.
> It distinguishes between what is implemented, what is defined, and what is not yet built.

**Related documents:**
- [Architecture](architecture.md)
- [Roadmap](roadmap.md)
- [AI Context](ai-context.md)

---

## Table of Contents

- [Status Summary](#status-summary)
- [System Reality Snapshot](#system-reality-snapshot)
- [Implemented](#implemented)
  - [Foundation Layer](#foundation-layer)
  - [Spec Layer](#spec-layer)
- [Defined but Not Implemented](#defined-but-not-implemented)
  - [Execution Layer](#execution-layer)
  - [Core Runtime Architecture](#core-runtime-architecture)
  - [Capability System](#capability-system)
  - [Agent Layer](#agent-layer)
  - [Deterministic Analysis Layer](#deterministic-analysis-layer)
  - [Policy and Governance](#policy-and-governance)
- [Current Workflow Reality](#current-workflow-reality)
- [Current Architectural Alignment](#current-architectural-alignment)
- [Gaps vs Target Architecture](#gaps-vs-target-architecture)
- [Known Risks](#known-risks)
- [Current Focus](#current-focus)
- [Immediate Next Steps](#immediate-next-steps)
- [Definition of Done - Phase 0](#definition-of-done---phase-0)
- [Summary](#summary)

---

## Status Summary

Specwright is currently in a **foundation-first, pre-runtime phase**.

The architectural direction is now clearly defined. Specwright is positioned as a **codebase-aware engineering operating system for AI-assisted teams**, centered on deterministic analysis, bounded context assembly, capability-driven execution, and policy-enforced workflow governance.

At this stage, the architecture is defined, but the runtime system is not yet implemented.

Today, Specwright is best described as:

- a well-defined architectural model
- a repository-centered documentation system
- a manual, human-guided workflow for spec-driven development

It is **not yet** a running platform with orchestration, execution journaling, policy enforcement, or CI-driven workflow validation.

---

## System Reality Snapshot

| Area | Status | Notes |
| --- | --- | --- |
| Product vision | Defined | Positioned as a codebase-aware engineering operating system |
| Foundation layer | Established | `architecture.md`, `current-state.md`, `roadmap.md`, `ai-context.md` exist |
| Spec layer | Established | `spec.md` and `implementation-notes.md` workflow exists conceptually |
| Execution layer | Not implemented | Workflows, capabilities, agents, retrieval, and policies are not yet running |
| Runtime baseline | Not implemented | `.NET 10 + Aspire + PostgreSQL` is defined, not scaffolded |
| Deterministic-first rule | Defined | Architectural rule exists, but no implementation enforces it yet |
| Capability model | Defined conceptually | No capability interfaces or host yet |
| Agent model | Defined conceptually | No executable agent runtime exists |
| Policy/governance | Defined conceptually | No enforcement engine exists |
| Impact-based workflow rigor | Defined | L1/L2/L3 model exists architecturally, not operationally |
| CI/PR integration | Not implemented | Planned only |
| Brownfield analysis support | Not implemented | Analyzer direction exists, but no analyzer runtime yet |

---

## Implemented

### Foundation Layer

The following foundation documents exist and currently act as the primary project memory:

- `docs/architecture.md`
- `docs/current-state.md`
- `docs/roadmap.md`
- `docs/ai-context.md`

These documents currently serve as:

- source-of-truth references for humans and AI
- architectural guidance
- workflow guidance
- system constraints and conventions

They are first-class project artifacts, but they are not yet backed by automated validation.

### Spec Layer

The spec layer is defined through the use of:

- `spec.md`
- `implementation-notes.md`

This layer supports:

- design accountability
- implementation accountability
- structured documentation of deviations, tradeoffs, and outcomes

This workflow exists today as a documented practice, not an enforced runtime behavior.

---

## Defined but Not Implemented

### Execution Layer

The architecture defines an execution layer made up of:

- workflows
- capabilities
- agents
- retrieval
- policy evaluation
- CI and PR gates

This layer is central to the target system but does not yet exist as executable software.

### Core Runtime Architecture

The architecture defines the following runtime components:

- API entry points
- workflow orchestrator
- capability host
- context assembler
- policy and governance engine
- execution journal
- PostgreSQL persistence
- worker support for async or long-running jobs

Current state:

- none of these runtime components are scaffolded or implemented
- the solution structure under `src/` and `tests/` does not yet represent the target system

### Capability System

The architecture explicitly favors **capabilities over role blobs**.

Expected examples include:

- `analyze_repo_structure`
- `analyze_diff`
- `extract_symbols`
- `assemble_context_packet`
- `validate_spec_traceability`
- `validate_required_artifacts`

Current state:

- no capability interfaces exist
- no capability contracts exist
- no capability composition rules exist in code
- no capability host exists

### Agent Layer

The architecture defines the following agents:

- Architect
- Engineer
- Reviewer
- Analyzer

Their purpose is clear within the system model, but they currently exist only as conceptual roles and workflow responsibilities.

Current state:

- no agent execution model
- no agent orchestration model
- no agent-to-capability composition implementation
- no bounded agent invocation pipeline

### Deterministic Analysis Layer

The architecture establishes a core rule:

> deterministic analysis before model reasoning

Planned deterministic sources include:

- Roslyn-based code analysis
- symbol extraction
- diff analysis
- markdown fact extraction
- lexical and metadata retrieval
- bounded evidence assembly

Current state:

- no deterministic analysis pipeline exists
- no Roslyn-based analyzer exists
- no codebase inventory capability exists
- no bounded context packet generator exists

### Policy and Governance

The architecture defines a policy and governance model responsible for:

- role and capability policy
- impact-level routing
- approval paths
- CI and PR gate decisions
- evidence and artifact checks

Current state:

- no policy model exists in code
- no policy engine exists
- no automated workflow validation exists
- no approval or routing behavior is enforced

---

## Current Workflow Reality

Today, Specwright operates like this:

```text
Human + AI -> follow documented process -> produce artifacts manually
```
It does not yet operate like this:

```text
System -> orchestrates capabilities -> assembles bounded context -> invokes agents -> validates outputs -> records evidence -> enforces workflow outcomes
```

---

## Current Architectural Alignment

The current documentation is aligned with the architecture in the following ways:

- Specwright is framed as a system, not just a template repo
- foundation docs are treated as operational memory
- spec and implementation documents are treated as traceable artifacts
- deterministic-first processing is the governing design rule
- Aspire is the intended orchestration baseline
- PostgreSQL is the intended primary store
- agents are defined as orchestrations over capabilities
- workflow rigor is intended to vary by change impact
- the platform is designed to integrate with PR and CI workflows rather than replace them

What is missing is not architectural clarity, but implementation.

---

## Gaps vs Target Architecture

| Target Capability                         | Current State   |
| ----------------------------------------- | --------------- |
| `.NET 10` runtime baseline                | Not implemented |
| Aspire orchestration                      | Not implemented |
| ASP.NET Core API entry points             | Not implemented |
| workflow orchestrator                     | Not implemented |
| capability host                           | Not implemented |
| bounded context packets                   | Not implemented |
| deterministic analysis pipeline           | Not implemented |
| PostgreSQL persistence                    | Not implemented |
| execution journal                         | Not implemented |
| policy and governance engine              | Not implemented |
| impact-based workflow routing             | Not implemented |
| CI/PR policy gates                        | Not implemented |
| analyzer support for brownfield codebases | Not implemented |

---

## Known Risks

**Risk 1 - Documentation Leads Runtime by Too Wide a Margin**

The architecture is well-defined, but the runtime does not yet exist.

Impact: High
Status: Open

**Risk 2 - Spec Drift**

Without automated checks, specs, implementation notes, and foundation docs can drift from actual implementation.

Impact: High
Status: Open

**Risk 3 - Human Discipline Dependency**

The current workflow depends on people remembering to:

update the right documents
maintain traceability
challenge incomplete specs
follow architectural rules manually

Impact: High
Status: Open

**Risk 4 - Capability Model Remains Abstract**

The architecture strongly depends on a capability-first design, but that design has not yet been translated into contracts or code boundaries.

Impact: Medium
Status: Open

**Risk 5 - No Enforcement of Change Impact Model**

The L1/L2/L3 model exists architecturally, but there is no operational workflow routing or approval behavior yet.

Impact: Medium
Status: Open

---

### Current Focus

**Phase 0 - Foundation Stabilization**

The current focus is to move from:

- coherent architecture
- manual workflow discipline
- repository-based documents

to:

- explicit runtime contracts
- scaffolded system boundaries
- first executable workflow slice

This phase is about making the architecture buildable and reducing ambiguity before implementation begins.

---

### Immediate Next Steps
1. Define the initial capability model
   - interfaces
   - contracts
   - composition rules
   - execution boundaries
2. Define the bounded context packet schema
   - scope
   - evidence
   - unknowns
   - constraints
   - token budget
3. Define the initial policy model
   - rule types
   - routing logic
   - enforcement points
   - workflow outcomes
4. Scaffold the .NET 10 + Aspire solution
   - Specwright.AppHost
   - Specwright.ServiceDefaults
   - Specwright.Api
   - Specwright.Core
   - Specwright.Infrastructure
   - Specwright.Worker
   - test projects
5. Introduce PostgreSQL as the primary system store
6. Implement the first end-to-end vertical slice
   - deterministic analysis
   - bounded context assembly
   - output generation
   - execution persistence

---

Definition of Done - Phase 0

Phase 0 is complete when:

 - [ ]capability model is documented
 - [ ] context packet schema is documented
 - [ ] policy model is documented
 - [ ] solution structure is scaffolded
 - [ ] Aspire baseline is in place
 - [ ] PostgreSQL is wired into the runtime
 - [ ] project boundaries are established
 - [ ] first workflow can execute end-to-end
 - [ ] execution results can be persisted
 - [ ] foundation documents reflect actual scaffold decisions

---

## Summary

Specwright is currently an architecturally aligned but pre-runtime system.

It already has:

- a clearly defined product vision
- a strong foundation layer
- a spec-driven documentation model
- a deterministic-first architectural rule
- a capability-first execution philosophy
- a planned Aspire and PostgreSQL runtime baseline

It does not yet have:

- executable runtime components
- deterministic analysis infrastructure
- policy enforcement
- workflow orchestration
- execution journaling
- CI or PR integration
- brownfield analyzer workflows

The project is currently in the transition between architecture definition and platform implementation.