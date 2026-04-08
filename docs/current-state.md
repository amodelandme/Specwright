# Current State - Specwright

**Last updated:** 2026-04-08
**Updated by:** AI assistant + user direction
**Current phase:** Phase 0 - Platform Reset and Foundation Setup

> This is the single source of truth for where the project stands right now.
>
> Guardrails and conventions: [`docs/ai-context.md`](ai-context.md)

---

## Status Summary

**Phase 0 - Platform Reset: in progress**

The project direction has shifted from framework-focused templates to a runnable `.NET 10 + Aspire` platform. Initial architecture intent has been captured in draft form and renamed to `new-Specwright-arch-draft.md`. The next critical step is solution bootstrap with Aspire and Postgres, then implementation of the first vertical workflow slice.

---

## Environment Status

| Environment | Status | Notes |
|---|---|---|
| Local dev | In progress | Repo exists, doc foundation now initialized |
| CI pipeline | Not started | CI gates for build/test/policy not configured yet |
| Staging | Not started | Deferred |
| Production | Not started | Deferred |

---

## What Is Completed

### Project Direction and Foundation

- New strategic direction established: `.NET 10`, Aspire, PostgreSQL.
- Initial architecture draft written and retained as `new-Specwright-arch-draft.md`.
- Foundation docs initialized under `docs/`.
- Initial execution checklist and phase framing defined.

---

## What Is Not Yet Built

### Solution and Runtime

- [ ] Aspire solution scaffold (`AppHost`, `ServiceDefaults`, API, core projects)
- [ ] PostgreSQL resource wiring in AppHost
- [ ] API/worker database connectivity via Aspire references

### Core Platform Capabilities

- [ ] Deterministic analyzer capability (repo/project/symbol facts)
- [ ] Context assembler capability (bounded context packets)
- [ ] Retrieval capability (metadata + lexical-first)
- [ ] Policy engine MVP (required artifacts + evidence checks)
- [ ] Execution journal persistence and query APIs

### Testing and CI

- [ ] Unit test baseline
- [ ] Integration test baseline
- [ ] CI workflow for restore/build/test/format/policy gates

---

## Known Issues

### KI-001 - Runtime scaffold not yet created
**Severity:** High  
**Status:** Open

The architecture direction is defined, but no executable platform exists yet for validation. This blocks practical iteration on workflows and policy behavior.

**Planned fix:** Complete Phase 0 bootstrap tasks before starting capability implementation.

---

### KI-002 - Foundation docs still need active synchronization with implementation
**Severity:** Medium  
**Status:** Open

The docs are initialized but will drift if they are not updated as soon as scaffold and first workflows land.

**Planned fix:** Treat doc updates as completion criteria in every phase and enforce via CI later.

---

## Current Focus

**Phase 0 - Platform Reset and Foundation Setup**

### Immediate Next Tasks

1. Scaffold `.NET 10` Aspire solution and core projects under `src/`.
2. Add PostgreSQL hosting integration in AppHost and wire consuming projects.
3. Establish baseline project references and dependency rules.
4. Add initial test projects under `tests/`.
5. Implement minimal analyzer workflow vertical slice end-to-end.

---

## What Not To Do Right Now

- Do not split into many deployable services yet; start with modular boundaries in one solution.
- Do not add semantic retrieval as default; keep metadata + lexical-first.
- Do not add broad model-provider coupling; preserve model-agnostic interfaces.
- Do not postpone documentation updates; foundation docs are part of delivery.

---

## Definition of Done - Phase 0

- [ ] Aspire solution scaffolded and running locally
- [ ] PostgreSQL reachable through Aspire wiring
- [ ] Core project boundaries in place (`Api`, `Core`, `Infrastructure`, optional `Worker`)
- [ ] Tests compile and run in baseline form
- [ ] First vertical workflow can execute and persist a minimal run record
- [ ] Foundation docs updated to reflect actual scaffold decisions

---

## Lessons Learned

- Strategic pivots are cheaper when architecture intent is documented before scaffolding.
- Deterministic-first boundaries need to be encoded as implementation constraints early.
