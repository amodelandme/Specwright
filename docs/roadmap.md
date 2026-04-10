# Roadmap - Specwright

**Last updated:** 2026-04-09

> This roadmap describes how Specwright evolves from a documentation-centered workflow into a deterministic, policy-enforced engineering system for AI-assisted teams.

**Related documents:**
- [Architecture](architecture.md)
- [Current State](current-state.md)
- [AI Context](ai-context.md)

---

## Table of Contents

- [Roadmap Overview](#roadmap-overview)
- [Phase Map](#phase-map)
- [Phase Dependencies](#phase-dependencies)
- [Phase 0 - Foundation Stabilization](#phase-0---foundation-stabilization)
- [Phase 1 - Runtime Bootstrap](#phase-1---runtime-bootstrap)
- [Phase 2 - Capability System and Analyzer Slice](#phase-2---capability-system-and-analyzer-slice)
- [Phase 3 - Agent Layer](#phase-3---agent-layer)
- [Phase 4 - Policy and Governance Engine](#phase-4---policy-and-governance-engine)
- [Phase 5 - CI/PR Workflow Integration](#phase-5---cipr-workflow-integration)
- [Phase 6 - Aspire Operational Baseline](#phase-6---aspire-operational-baseline)
- [Phase 7 - Brownfield Intelligence](#phase-7---brownfield-intelligence)
- [Future Considerations](#future-considerations)
- [Summary](#summary)

---

## Roadmap Overview

Specwright is being developed in phases so the system grows in the correct order.

The roadmap prioritizes:

1. defining system contracts before implementation
2. establishing internal boundaries before adding complexity
3. implementing deterministic capabilities before model-heavy workflows
4. introducing policy enforcement only after workflows are real
5. integrating with existing engineering workflows rather than replacing them

This roadmap follows the architecture directly. It is meant to keep the runtime aligned with the system’s core principles:

- deterministic-first processing
- bounded context by default
- markdown source-of-truth artifacts
- capabilities over role blobs
- policy-enforced discipline
- adaptive governance
- workflow integration over replacement

---

## Phase Map

```text
Phase 0  🔄  Foundation stabilization + system definition          Q2 2026
Phase 1  ⏳  Runtime bootstrap (.NET 10 + Aspire + PostgreSQL)     Q2 2026
Phase 2  ⏳  Capability system + analyzer vertical slice           Q2 2026
Phase 3  ⏳  Agent layer (Architect / Engineer / Reviewer)         Q3 2026
Phase 4  ⏳  Policy and governance engine                          Q3 2026
Phase 5  ⏳  CI/PR workflow integration                            Q3-Q4 2026
Phase 6  ⏳  Aspire operational baseline and service composition   Q4 2026
Phase 7  ⏳  Brownfield intelligence and onboarding support        Q4 2026+
```

> Status key: ✅ Complete  🔄 In Progress  ⏳ Not Started  🚫 Blocked
>
> Milestone targets are rough quarters, not commitments. Update them when scope
> or priorities shift — a stale date is worse than no date.

---

## Phase Dependencies

| Phase   | Depends On | Why                                                              |
| ------- | ---------- | ---------------------------------------------------------------- |
| Phase 1 | Phase 0    | Runtime scaffold should follow explicit contracts and boundaries |
| Phase 2 | Phase 1    | Capabilities need a host, persistence, and runtime structure     |
| Phase 3 | Phase 2    | Agents should compose capabilities, not replace them             |
| Phase 4 | Phase 3    | Governance becomes meaningful once workflows and agents exist    |
| Phase 5 | Phase 4    | CI and PR enforcement requires executable policy checks          |
| Phase 6 | Phase 1    | Aspire composes services after the runtime shape exists          |
| Phase 7 | Phase 2    | Brownfield intelligence depends on analyzer capabilities         |

---

### Phase 0 - Foundation Stabilization

**Status:** In Progress
**Goal:** Make the architecture directly buildable
**Success Metric:** The system is defined clearly enough to scaffold without guessing

**Objectives:**
- align foundation docs to the architecture
- define the initial capability model
- define the bounded context packet schema
- define the initial policy model
- make the runtime shape explicit before implementation begins
**Deliverables:**
- synchronized foundation documents
- initial capability contracts
- context packet schema
- initial policy model
- clear src/ and tests/ boundary plan
**Exit Criteria:**
- [ ]foundation docs are synchronized
- [ ]capability model is documented
- [ ]context packet schema is documented
- [ ]policy model is documented
- [ ]runtime bootstrap path is explicit enough to execute directly

---

### Phase 1 - Runtime Bootstrap

**Status:** Not Started
**Goal:** Establish the first executable platform baseline
**Success Metric:** A minimal .NET 10 + Aspire + PostgreSQL solution runs locally with clear internal boundaries

**Objectives:**
- create the solution structure under src/
- add AppHost and ServiceDefaults
- add Api, Core, Infrastructure, and Worker projects
- establish initial project reference rules
- add PostgreSQL integration
- add baseline unit and integration tests

**Deliverables:**
- runnable solution scaffold
- baseline Aspire composition
- PostgreSQL wiring
initial API entry point
- initial execution journal persistence shape
- baseline test projects

**Exit Criteria:**
- [ ] solution runs locally
- [ ] PostgreSQL is reachable through runtime wiring
- [ ] project boundaries are reflected in references
- [ ] tests compile and run
- [ ] a minimal workflow request can be executed and persisted

---

### Phase 2 - Capability System and Analyzer Slice

**Status:** Not Started
**Goal:** Implement the first deterministic workflow slice
**Success Metric:** One analyzer workflow executes end-to-end using explicit capabilities and bounded context assembly

**Objectives:**
- implement the first capability interfaces
- build deterministic repo and project analysis
- build context assembly from bounded evidence
- persist workflow execution records
- return structured, evidence-backed workflow outputs

**Candidate Initial Capabilities:**
- analyze_repo_structure
- analyze_project_inventory
- extract_symbols
- assemble_context_packet
- record_execution

**Deliverables:**
- capability interfaces and contracts
- first capability host behavior
- analyzer workflow endpoint or command path
- execution journal records
- tests validating deterministic behavior

**Exit Criteria:**
- [ ] at least one capability flow runs end-to-end
- [ ] outputs include bounded evidence
- [ ] execution results are persisted
- [ ] tests cover the vertical slice
- [ ] deterministic analysis occurs before any model reasoning

---

### Phase 3 - Agent Layer

**Status:** Not Started
**Goal:** Introduce bounded agent workflows on top of the capability system
**Success Metric:** Agents compose capabilities while remaining inspectable, constrained, and auditable

**Objectives:**
- define agent-to-capability composition
- introduce Architect workflow behavior
- introduce Engineer workflow behavior
- introduce Reviewer workflow behavior
- introduce Analyzer workflow behavior

**Deliverables:**
- explicit agent composition rules
- bounded invocation model
- clear separation between deterministic steps and reasoning steps
- agent outputs tied to context packets and evidence

**Exit Criteria:**
- [ ] agents operate through capabilities rather than hidden logic
- [ ] Reviewer is represented explicitly
- [ ] agent outputs are bounded and auditable
- [ ] reasoning steps are grounded in deterministic inputs

---

### Phase 4 - Policy and Governance Engine

**Status:** Not Started
**Goal:** Turn workflow expectations into executable governance
**Success Metric:** The system can evaluate workflow outcomes against rules and produce enforceable pass/fail results

**Objectives:**
- implement required artifact checks
- implement evidence and citation checks
- implement impact-based routing for L1, L2, and L3 changes
- implement initial approval path logic
- implement initial traceability checks

**Candidate Policy Types:**
- missing required artifacts
- missing implementation notes
- missing supporting evidence
- invalid workflow path for impact level
- incomplete spec-to-implementation mapping
- missing foundation updates for architectural changes

**Deliverables:**
- policy model in code
- governance engine pipeline
- structured policy outcomes
- deterministic validation overlays for reviewer workflows

**Exit Criteria:**
- [ ] policy checks run as part of workflows
- [ ] outputs include pass/fail outcomes
- [ ] at least one impact-aware path is operational
- [ ] reviewer validation incorporates policy and evidence results

---

### Phase 5 - CI/PR Workflow Integration

**Status:** Not Started
**Goal:** Integrate Specwright into repository-based engineering workflows
**Success Metric:** Pull requests and CI runs can be checked against Specwright workflow and policy rules

**Objectives:**
- integrate workflow checks into CI
- validate documentation updates for relevant change types
- surface review results in developer-friendly outputs
- preserve PR-based development as the primary code change unit
- add policy gates for selected workflow outcomes

**Deliverables:**
- CI workflow integration
- PR-facing validation outputs
- machine-readable and human-readable review results
- initial workflow enforcement strategy

**Exit Criteria:**
- [ ] CI can execute workflow and policy checks
- [ ] failing policy states can block selected change paths
- [ ] PR workflows remain understandable and usable
- [ ] Specwright augments existing workflows rather than replacing them

### Phase 6 - Aspire Operational Baseline

**Status:** Not Started
**Goal:** Establish Specwright’s operational backbone for local-first distributed evolution
**Success Metric:** The platform runs as a composed Aspire application with service discovery, health, and diagnostics support

**Objectives:**
- fully wire AppHost and ServiceDefaults
- compose API, worker, database, and supporting services
- standardize configuration and health checks
- improve telemetry and diagnostics
- support future distributed evolution without premature service sprawl

**Deliverables:**
- Aspire application composition
- shared service defaults
- health check baseline
- telemetry and diagnostics baseline
- local development topology aligned with future runtime shape

**Exit Criteria:**
- [ ] runtime services are composed through Aspire
- [ ] health and diagnostics are visible
- [ ] local development reflects future system topology
- [ ] the platform can evolve without immediate service fragmentation

---

### Phase 7 - Brownfield Intelligence

**Status:** Not Started
**Goal:** Make Specwright useful for existing codebases, not only greenfield systems
**Success Metric:** The system can analyze an existing repository and produce useful, evidence-backed architectural understanding

**Objectives:**
- expand analyzer capabilities for brownfield scanning
- generate architecture understanding from codebases
- support onboarding-oriented outputs
- support evidence-backed artifact generation
- improve retrieval for large codebases where needed

**Deliverables:**
- brownfield analysis workflows
- repository understanding outputs
- onboarding summaries
- architecture discovery artifacts
- optional retrieval enhancements where lexical retrieval is insufficient

**Exit Criteria:**
- [ ] existing repositories can be analyzed meaningfully
- [ ] outputs are grounded in deterministic evidence
- [ ] generated artifacts support onboarding and system discovery
- [ ] brownfield support becomes a first-class platform capability


### Future Considerations

The following items remain future-facing and should only be pursued when justified by real system needs:

- semantic retrieval where lexical and metadata retrieval are insufficient
- selective extraction of independent services from core modules
- expanded policy automation for architecture rule compliance
- richer review intelligence
- packaging and distribution strategies for long-term adoption

These items should follow proven boundaries, not precede them.

---

### Summary

The roadmap moves Specwright through a deliberate sequence:

1. define the system
2. scaffold the runtime
3. implement deterministic capabilities
4. compose bounded agents
5. enforce governance and policy
6. integrate with CI and PR workflows
7. expand into brownfield intelligence

This order follows the architecture directly.

Specwright should not become an agent demo with loose documentation attached.
It should become a disciplined engineering platform whose runtime behavior matches its design principles.
