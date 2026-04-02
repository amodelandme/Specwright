# Current State — <ProjectName>

**Last updated:** <YYYY-MM-DD>
**Updated by:** <Name>
**Current phase:** Phase [X] — [Phase Name]

> This is a living document. Update it when work is completed, phases shift,
> or known issues are opened or resolved. It is the single source of truth for
> "where things stand right now."
>
> Guardrails and conventions: [`docs/ai-context.md`](ai-context.md)

---

## Table of Contents

- [Status Summary](#status-summary)
- [Environment Status](#environment-status)
- [What Is Completed](#what-is-completed)
- [What Is Not Yet Built](#what-is-not-yet-built)
- [Known Issues](#known-issues)
- [Current Focus](#current-focus)
- [What Not To Do Right Now](#what-not-to-do-right-now)
- [Definition of Done — Phase X](#definition-of-done--phase-x)
- [Lessons Learned](#lessons-learned)

---

## 📍 Status Summary

**Phase 0 — Foundation: ✅ Complete**
**Phase 1 — [Phase Name]: 🔄 In Progress**
**Phase 2 — [Phase Name]: ⏳ Not Started**

[2–4 sentences describing the overall project state right now. What was most
recently completed? What is actively in progress? What is the most important
open question or blocker? This should be the first thing someone reads when
they sit down to work on this project.]

> **Example:** Domain layer and EF Core persistence are complete. CI pipeline is live
> with format and build gates. Validation and sanitization are in progress on the
> current feature branch. The next milestone is a passing full test suite before
> Phase 2 begins.

---

## 🖥️ Environment Status

| Environment | Status | Notes |
|---|---|---|
| Local dev | ✅ Working | [e.g., `docker compose up` → service ready at `https://localhost:5001`] |
| CI pipeline | ✅ Passing | [e.g., `lint-format` and `build-test` jobs green on `dev` and `main`] |
| Staging | ⏳ Not yet | [e.g., Deferred to Phase X — Azure Container Apps deployment] |
| Production | ⏳ Not yet | [e.g., Deferred to Phase X] |

---

## ✅ What Is Completed

> Group completed work by phase or functional area. Name the specific files,
> classes, and PRs. This section should be specific enough that a new contributor
> can verify the state of the codebase by reading it.

### Phase 0 — Foundation ✅

- [Completed item — e.g., "`Order` entity with controlled mutation — private setters, explicit `Update()` method"]
- [Completed item — e.g., "`IOrderRepository` interface defined in Domain"]
- [Completed item — e.g., "EF Core + [provider] persistence — `OrderDbContext`, Fluent API configuration"]
- [Completed item — e.g., "Clean Architecture project structure: Domain, Application, Infrastructure, Api, Tests"]
- [Completed item — e.g., "`docker-compose.yml` — one-command local [database] setup"]

### Phase 1 — [Phase Name] ✅ (or 🔄 In Progress)

- [Completed item — e.g., "FluentValidation v12 — all request DTOs validated; manual `ValidateAsync` in controllers (PR #XX)"]
- [Completed item — e.g., "`GlobalExceptionMiddleware` — single catch-all; domain exceptions → named 4xx; unexpected → safe 500 (PR #XX)"]
- [In-progress item with context]

---

## ❌ What Is Not Yet Built

> List remaining work grouped by area. Items here drive the Current Focus
> section below. Keep this honest — it is more useful than a clean roadmap.

### [Area 1 — e.g., Testing]

- [ ] [Specific pending task — e.g., "Unit tests for `[ComponentName]` — dispatch, edge cases, fail-closed behavior"]
- [ ] [Specific pending task]

### [Area 2 — e.g., Developer Experience]

- [ ] [e.g., "`.http` smoke test request file committed to repo"]
- [ ] [e.g., "Seed data for local development environment"]

### [Area 3 — e.g., Error Handling]

- [ ] [e.g., "Route parameter guard for `{id}` on GET/PUT — closes KI-001"]

---

## ⚠️ Known Issues

> Each issue should have an ID, severity, status, a plain-English description
> of the problem, and either a workaround or a planned fix.
> Close issues here when they are resolved — do not delete them.

### KI-001 — [Issue Title]

**Severity:** [Low / Medium / High]
**Status:** [Open / Mitigated — workaround in place / Deferred to Phase X / Closed — PR #XX]

[Plain-English description of the issue. What is the behavior? What is the risk
or consequence? What layers or components are affected?]

**Workaround / Planned fix:** [If mitigated: the exact workaround. If deferred:
the planned approach and what phase it belongs to. If open: what investigation
is needed.]

---

### KI-002 — [Issue Title]

**Severity:** [Low / Medium / High]
**Status:** [Open]

[Same pattern]

**Workaround / Planned fix:** [...]

---

### Spec Writing — Lessons Applied

> When a spec produces a known pattern of gaps or ambiguities, document it here
> so future specs can avoid the same issues. This is not a bug — it is a process
> signal.

**[Pattern name — e.g., "Always enumerate all affected methods in service-layer ACs"]:**
[What the gap was, what was missed, and the rule going forward.]

---

## 🎯 Current Focus

**Phase [X] — [Phase Name]**

> This section is the authoritative source for "what are we working on right now."
> `roadmap.md` references here rather than maintaining its own current focus.

### Immediate Next Tasks

1. [Task 1 — specific and actionable. e.g., "Write unit tests for `[StrategyName]` — happy path, null config, out-of-range values"]
2. [Task 2]
3. [Task 3]
4. [Task 4]

---

## 🧭 What Not To Do Right Now

> Explicit guardrails for this moment in development. These exist because the
> temptation to do these things is real, and doing them now would cause problems.

- **No [feature category] yet ([Phase X])** — [Why: what it depends on that doesn't exist yet]
- **Do not change `[config value / setting]`** — [Why: what it is required for]
- **Do not use `[deprecated package or pattern]`** — [What to use instead and where it's documented]
- **Do not add `try/catch` to controllers** — `GlobalExceptionMiddleware` handles all exceptions; controllers contain only the happy path
- **[Add any project-specific constraint that someone might plausibly ignore]**

---

## 📌 Definition of Done — Phase [X]

> Check items off as they are completed. Unchecked items drive Current Focus.
> When all items are checked, the phase is done and roadmap.md should be updated.

- [x] [Completed item — be specific enough to verify]
- [x] [Completed item]
- [ ] [Incomplete item]
- [ ] [Incomplete item]
- [ ] `dotnet build` → 0 errors, 0 warnings
- [ ] All unit tests passing: `dotnet test --filter "Category!=Integration"`
- [ ] [Code formatter] check passing — 0 violations

---

## 📚 Lessons Learned

> Process and technical insights that should shape how future specs are written
> and how future implementations are approached. These are patterns, not bugs.
> Write each entry for a future spec author reading it cold — not for yourself
> reading it today.

### [YYYY-MM-DD] — [Lesson Title]

[What happened. What was assumed. What was wrong or surprising. Keep it factual
and non-blaming.]

**The rule going forward:** [The convention or process change this produces.
Future specs and implementations should follow this.]

> **Example:**
> When a spec instructs updating service methods that throw or return null,
> every affected method must be explicitly listed by name in the acceptance
> criteria. In PR #XX, `[MethodName]` was omitted from the list but caught
> during implementation via the DoD. The method was updated correctly, but
> the gap created unnecessary ambiguity.
>
> **The rule:** Acceptance criteria that touch "all methods that do X" must
> enumerate every method by name. Do not rely on "and any similar methods."
