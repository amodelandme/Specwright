# Roadmap — <ProjectName>

**Last updated:** <YYYY-MM-DD>

> **Product vision:** See [`docs/architecture.md — Product Vision`](architecture.md#product-vision).
> **Current focus:** See [`docs/current-state.md — Current Focus`](current-state.md#current-focus).
> **Guardrails:** See [`docs/ai-context.md`](ai-context.md).

---

## Table of Contents

- [Phase Map](#phase-map)
- [Phase Dependencies](#phase-dependencies)
- [Phase 0 — Foundation](#phase-0--foundation)
- [Phase 1 — [Phase Name]](#phase-1--phase-name)
- [Phase 2 — [Phase Name]](#phase-2--phase-name)
- [Phase 3 — [Phase Name]](#phase-3--phase-name)
- [Phase 4 — [Phase Name]](#phase-4--phase-name)
- [Phase 5 — [Phase Name]](#phase-5--phase-name)
- [Long-Term Vision](#long-term-vision)

---

## 🗺️ Phase Map

```
Phase 0  ✅  Foundation — core domain, persistence, basic API
Phase 1  🔄  [Phase Name] — [one-line description]           Q[X] YYYY
Phase 2  ⏳  [Phase Name] — [one-line description]           Q[X] YYYY
Phase 3  ⏳  [Phase Name] — [one-line description]           Q[X] YYYY
Phase 4  ⏳  [Phase Name] — [one-line description]           Q[X] YYYY
Phase 5  ⏳  [Phase Name] — [one-line description]           Q[X] YYYY
```

> Status key: ✅ Complete  🔄 In Progress  ⏳ Not Started  🚫 Blocked
>
> Milestone targets are rough quarters, not commitments. Update them when
> scope or priorities shift — a stale date is worse than no date.

---

## Phase Dependencies

> Some phases have hard prerequisites. Document them here so the phase order
> is explicit and the reason for it is not lost.

| Phase | Blocked By | Why |
|---|---|---|
| Phase 3 — [Auth] | Phase 1 complete | [e.g., Rate limiting and audit logging require caller identity] |
| Phase 4 — [Observability] | Phase 3 complete | [e.g., Audit log requires identity from Phase 3] |
| Phase [X] | Phase [Y] complete | [Reason] |

---

## 🧱 Phase 0 — Foundation ✅ Complete

> **Success metric:** The core domain model is correct, persistence is working,
> and the API returns valid responses end-to-end on local dev.

### Core Domain and Architecture

* [x] [Define `[Entity]` domain entity with controlled mutation]
* [x] [Define enums, value objects, domain interfaces]
* [x] [Enforce Clean Architecture project structure and dependency directions]

### Application Layer

* [x] [Define `I[Feature]Service` interface — async, CancellationToken throughout]
* [x] [Implement core application service]
* [x] [Define DTOs: request, response, mappings]

### API Layer

* [x] [`[Entity]Controller` — core CRUD endpoints]
* [x] [OpenAPI / Swagger setup]

### Infrastructure and Persistence

* [x] [EF Core + [provider] setup]
* [x] [`[Entity]Repository` — async, full CRUD]
* [x] [Initial migrations — baseline schema]
* [x] [`docker-compose.yml` — one-command local [database] setup]

### Dev Environment

* [x] [DevContainer or local dev setup documented]
* [x] [`.config/dotnet-tools.json` — tool manifest]

---

## 🔄 Phase 1 — [Phase Name] (Current Focus)

> **Success metric:** [One sentence describing what "done" looks like at the
> outcome level. e.g., "All API endpoints are validated, the CI pipeline blocks
> on format and build failures, and the full unit test suite is passing."]

> **Note:** [If any work was pulled forward or deferred from its original phase,
> document it here with the rationale. This preserves the decision history without
> requiring readers to dig through git log.
> Example: "Integration tests were originally scoped to this phase but moved to
> Phase 2 — they require a Postgres service container in CI that isn't configured yet."]

### [Area 1 — e.g., Validation and Sanitization] ✅ Complete (PR #XX)

* [x] [Specific completed task]
* [x] [Specific completed task]

### [Area 2 — e.g., CI Pipeline] ✅ Complete (PR #XX)

* [x] [Specific completed task]
* [x] [Specific completed task]

### [Area 3 — e.g., Testing] 🔄 In Progress

* [ ] [Specific pending task]
* [ ] [Specific pending task]
* [ ] [Specific pending task]

### [Area 4 — e.g., Developer Experience]

* [ ] [Specific pending task]
* [ ] [Specific pending task]

---

## ⏳ Phase 2 — [Phase Name]

> **Success metric:** [One sentence. What does success look like at the outcome
> level? How would you know this phase is done enough to move on?]

* [ ] [Task]
* [ ] [Task]
* [ ] [Task]

---

## ⏳ Phase 3 — [Phase Name]

> **Success metric:** [...]
>
> **Depends on:** Phase 2 complete — [reason]

* [ ] [Task]
* [ ] [Task]
* [ ] [Task]

---

## ⏳ Phase 4 — [Phase Name]

> **Success metric:** [...]
>
> **Depends on:** Phase 3 complete — [reason]

* [ ] [Task]
* [ ] [Task]
* [ ] [Task]

---

## ⏳ Phase 5 — [Phase Name]

> **Success metric:** [...]

* [ ] [Task]
* [ ] [Task]
* [ ] [Task]

---

## 🗺️ Long-Term Vision

[2–4 sentences describing where this system could go beyond the current roadmap.
Keep this aspirational but grounded — it should be a plausible extension of what
is being built, not a product pivot. This section is useful context for
architectural decisions made today that need to accommodate tomorrow's direction.]

> **Example:** Full observability and experimentation platform for [target ecosystem].
> A/B testing capabilities with statistical significance reporting. AI-powered
> [feature] lifecycle management. Real-time [metric] dashboards. Open core plus
> managed hosting business model.
