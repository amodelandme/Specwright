# <Feature Name> — Implementation Notes

**Session date:** <YYYY-MM-DD>
**Branch:** `<feature/ | fix/ | refactor/...>`
**Spec reference:** `docs/decisions/<feature-branch-name>/spec.md`
**Build status:** Passed — 0 warnings, 0 errors
**Tests:** <X>/<X> passing
**PR:** `<TBD or #number>`

---

## Table of Contents

- [What Was Built](#what-was-built)
- [Spec Gaps Resolved](#spec-gaps-resolved)
- [Deviations from Spec](#deviations-from-spec)
- [Key Decisions](#key-decisions)
- [File-by-File Changes](#file-by-file-changes)
- [Risks and Follow-Ups](#risks-and-follow-ups)
- [How to Test](#how-to-test)
- [Interview Lens](#interview-lens)
- [Foundation Docs Updated](#foundation-docs-updated)
- [Definition of Done — Status](#definition-of-done--status)

---

## What Was Built

[2–4 sentences summarizing what was implemented. Write this as if you're
explaining it to a new team member who has not read the spec. Describe the
outcome — what the system now does that it didn't do before — not the steps taken.]

> **Example:** A domain exception hierarchy and global exception middleware that
> replaces all per-controller `try/catch` blocks. Every error response now returns
> a consistent `ProblemDetails` shape with `Content-Type: application/problem+json`.
> Controllers contain only the happy path.

---

## Spec Gaps Resolved

> Document every place where the spec was incomplete, ambiguous, or incorrect,
> and how it was resolved. This section feeds directly back into the spec-writing
> process — gaps found here improve future specs.
>
> If the spec was complete, write "None." Do not omit this section.

### Gap 1 — [Brief title]

[What the spec said or implied. What was missing or ambiguous. What the correct
behavior turned out to be. How it was resolved — was it confirmed with the spec
author, derived from other context, or independently reasoned?]

> **Example:** AC-6 listed three service methods to update (`GetAsync`,
> `UpdateAsync`, `ArchiveAsync`) but `IsEnabledAsync` also matched the same
> pattern. The DoD item "POST /api/evaluate returns ProblemDetails 404"
> confirmed the intent. `IsEnabledAsync` was updated along with the others.

---

### Gap 2 — [Brief title]

[Same pattern]

---

## Deviations from Spec

> Document every place where the implementation deliberately differs from
> what the spec specified. Include the reasoning. This is not a confession — it
> is accountability. A future maintainer reading the spec should be able to
> reconcile what was actually built.
>
> If there were no deviations, write "None."

### [Deviation title]

**Spec said:** [What the spec specified]

**What was implemented instead:** [What was actually built]

**Reasoning:** [Why the spec's approach was not followed. What the deviation
improves — correctness, performance, clarity, convention compliance.]

> **Example:**
> **Spec said:** Create `new JsonSerializerOptions { ... }` inline in `WriteProblemDetailsAsync`.
> **What was implemented:** `static readonly JsonSerializerOptions` field on the middleware class.
> **Reasoning:** `JsonSerializerOptions` construction builds internal metadata caches and is
> expensive when allocated per-call. This is a documented .NET performance anti-pattern.
> The behavior is identical — no functional change.

---

## Key Decisions

> Decisions made during implementation that were not specified in the spec,
> or where the spec left room for judgement. These are the choices a senior
> engineer makes that a junior engineer might not notice.

### [Decision title]

[What was decided. Why. What the alternative was and why it was not chosen.
Keep these grounded in what actually happened — not hypotheticals.]

---

### [Decision title]

[Same pattern]

---

## File-by-File Changes

### New Files

| File | Purpose |
|---|---|
| `<Layer>/<Subfolder>/<FileName>.cs` | [What this file does — one line] |
| `<Layer>/<Subfolder>/<FileName>.cs` | [What this file does — one line] |

### Modified Files

| File | Change |
|---|---|
| `<Layer>/<Subfolder>/<FileName>.cs` | [What changed and why — one line] |
| `<Layer>/<Subfolder>/<FileName>.cs` | [What changed and why — one line] |

---

## Risks and Follow-Ups

> Known limitations, deferred concerns, or work this implementation creates
> that should not be forgotten. Each item should have an owner or a phase reference.

| Risk / Follow-Up | Severity | Status | Notes |
|---|---|---|---|
| [e.g., "Integration tests for the 404 and 500 paths not yet written"] | Low | Deferred to Phase [X] | [Requires running [database/service] in CI] |
| [e.g., "`[ClassName]` has an implicit precondition not enforced by a guard"] | Low | Documented — tracked for review | [See KI-00X in `current-state.md`] |
| [Risk or follow-up] | [Severity] | [Status] | [Notes] |

---

## How to Test

> Concrete steps to verify that this feature works correctly. Include both
> happy path and error path verification. These should be runnable without
> reading the spec.

### Unit Tests

```bash
dotnet test --filter "Category=Unit&FullyQualifiedName~[TestClassName]"
```

[What the tests cover. What to look for in the output.]

### Manual / Smoke Test

```http
[METHOD] https://localhost:[port]/api/[route]
Content-Type: application/json

{
  "[field]": "[value]"
}
```

**Expected response:**
```json
{
  "[field]": "[expected value]"
}
```

[HTTP status: XXX]

### Error Path Verification

```http
[METHOD] https://localhost:[port]/api/[route-that-triggers-error]
```

**Expected response:**
```json
{
  "type": "about:blank",
  "title": "[Expected title]",
  "status": [expected status code],
  "detail": "[Expected detail message]",
  "instance": "/api/[route]"
}
```

[HTTP status: XXX, Content-Type: application/problem+json]

---

## Interview Lens

> **Recommended.** If this feature involves a meaningful engineering decision,
> describe how you would explain it in a technical interview. A concise,
> confident explanation of a real decision you made is more memorable than
> a description of what you built.

**The decision:** [What was decided]

**How to explain it:**

[2–4 sentences. Lead with the problem you were solving, not the technology.
State the tradeoff clearly. Name what you would do differently at a different
scale or with different constraints.]

> **Example:** "We replaced per-controller `try/catch` blocks with a single
> `GlobalExceptionMiddleware`. The problem with controller-level handling is that
> it's a cross-cutting concern — having it in six places means six places to get
> wrong and no single place to change. Middleware wraps the entire pipeline,
> including routing and other middleware, so a single handler covers every case.
> We also introduced a domain exception base class that carries an HTTP status code,
> which means adding a new error type never requires touching the middleware."

---

## Foundation Docs Updated

> Before closing this PR, update the foundation documents to reflect the new
> state of the system. Check each item when done.

- [ ] `docs/current-state.md` — moved completed tasks from "Not Yet Built" to "Completed"
- [ ] `docs/current-state.md` — updated Status Summary to reflect current phase state
- [ ] `docs/current-state.md` — opened or closed Known Issues affected by this work
- [ ] `docs/current-state.md` — updated Definition of Done checklist
- [ ] `docs/roadmap.md` — checked off completed phase tasks
- [ ] `docs/architecture.md` — updated if this feature introduced new layers, boundaries,
      patterns, or changed the security model
- [ ] `docs/ai-context.md` — updated if this feature introduced new conventions,
      deprecated patterns, or new tech stack quirks

---

## Definition of Done — Status

> Mirror the DoD from the spec and mark each item. Items that cannot be completed
> in this PR (e.g., integration tests requiring infrastructure not yet in CI) should
> be marked incomplete with a note explaining the blocker and where they will be closed.

- [x] [Completed DoD item from spec]
- [x] [Completed DoD item from spec]
- [ ] [Incomplete DoD item] — [Reason: e.g., "Requires Postgres service container in CI — deferred to Phase X"]
- [x] `dotnet build <SolutionName>.sln` → 0 errors, 0 warnings
- [x] All unit tests passing: `dotnet test --filter "Category!=Integration"`
- [x] [Code formatter] check → 0 violations
