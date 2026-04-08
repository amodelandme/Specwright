# Roadmap - Specwright

**Last updated:** 2026-04-08

> **Product vision:** See [`docs/architecture.md`](architecture.md).
> **Current focus:** See [`docs/current-state.md`](current-state.md).
> **Guardrails:** See [`docs/ai-context.md`](ai-context.md).

---

## Phase Map

```text
Phase 0  🔄  Platform reset + Aspire bootstrap              Q2 2026
Phase 1  ⏳  Analyzer vertical slice (deterministic-first)  Q2 2026
Phase 2  ⏳  Foundation generator + retrieval hardening      Q2-Q3 2026
Phase 3  ⏳  Policy engine + reviewer gate MVP              Q3 2026
Phase 4  ⏳  CI/PR enforcement + observability expansion     Q3-Q4 2026
Phase 5  ⏳  Modular extraction + packaging transition       Q4 2026+
```

> Status key: ✅ Complete  🔄 In Progress  ⏳ Not Started  🚫 Blocked

---

## Phase Dependencies

| Phase | Blocked By | Why |
|---|---|---|
| Phase 1 | Phase 0 complete | Need runnable platform baseline before workflow development |
| Phase 2 | Phase 1 complete | Foundation generation depends on analyzer outputs |
| Phase 3 | Phase 2 complete | Policy checks require stable artifacts and retrieval |
| Phase 4 | Phase 3 complete | CI gates need meaningful policy outcomes |
| Phase 5 | Phase 4 complete | Service decomposition should follow proven boundaries |

---

## Phase 0 - Platform reset + Aspire bootstrap (Current)

**Success metric:** A `.NET 10 + Aspire + PostgreSQL` solution runs locally with clear internal boundaries and baseline tests.

- [ ] Create `src/` solution structure with AppHost and ServiceDefaults.
- [ ] Add API and core runtime projects with dependency boundaries.
- [ ] Add PostgreSQL hosting integration and references.
- [ ] Add `tests/` baseline projects.
- [ ] Ensure local run path and health checks are operational.

---

## Phase 1 - Analyzer vertical slice

**Success metric:** One end-to-end analyzer workflow executes, records evidence, and returns actionable output.

- [ ] Build deterministic repo/project inventory capability.
- [ ] Build minimal diff-aware analysis capability.
- [ ] Persist workflow execution records in PostgreSQL.
- [ ] Expose analyzer workflow endpoint(s) and result contract.
- [ ] Add unit/integration tests for the slice.

---

## Phase 2 - Foundation generator + retrieval hardening

**Success metric:** Analyzer outputs can produce or update foundation docs with traceable evidence and bounded context.

- [ ] Implement context packet builder (goal, scope, evidence, unknowns, budget).
- [ ] Implement markdown artifact parser/indexer.
- [ ] Implement metadata + lexical-first retrieval layer.
- [ ] Generate/update foundation docs from workflow outputs.
- [ ] Track and present citation/evidence references per conclusion.

---

## Phase 3 - Policy engine + reviewer gate MVP

**Success metric:** Workflow outputs are validated against enforceable documentation and architecture policies.

- [ ] Implement required-artifact policy checks.
- [ ] Implement citation/evidence completeness checks.
- [ ] Implement spec-to-implementation traceability checks (MVP scope).
- [ ] Add reviewer gate workflow output with deterministic validation overlays.

---

## Phase 4 - CI/PR enforcement + observability expansion

**Success metric:** CI can block changes that fail policy/quality gates, and workflow outcomes are auditable.

- [ ] Integrate workflow checks into CI.
- [ ] Enforce minimum documentation updates for relevant change types.
- [ ] Expand execution journal fields (context size, retrieved artifacts, policy outcomes).
- [ ] Add operational dashboards/telemetry views.

---

## Phase 5 - Modular extraction + packaging transition

**Success metric:** Stable modules are extracted cleanly, and framework assets are moved to long-term destination.

- [ ] Evaluate module extraction candidates based on ownership/scale/runtime behavior.
- [ ] Extract components with versioned contracts where justified.
- [ ] Move legacy templates/examples/skills to long-term package or repository model.
- [ ] Maintain compatibility bridge during migration window.

---

## Long-Term Vision

Specwright evolves into a codebase-aware engineering operating platform where design intent, implementation evidence, and policy enforcement stay continuously aligned. The system remains deterministic-first and model-agnostic, with increasing automation for brownfield onboarding, review quality, and architecture compliance across team workflows.
