# Initial Capability Model and Context Packet Schema — Implementation Notes

**Session date:** 2026-04-10  
**Branch:** `docs/spec-refinement`  
**Spec reference:** `docs/specs/initial-capability-model/spec-v2.md`  
**Build status:** Not run — documentation/spec refinement only  
**Tests:** Not run — documentation/spec refinement only  
**PR:** N/A

---

## Table of Contents

- [What Was Built](#what-was-built)
- [Spec Gaps Resolved](#spec-gaps-resolved)
- [Deviations from Spec v2](#deviations-from-spec-v2)
- [File-by-File Changes](#file-by-file-changes)
- [Definition of Done — Status](#definition-of-done--status)
- [AI Reviewer System Prompt](#ai-reviewer-system-prompt)

---

## What Was Built

A revised implementation-ready draft of the **Initial Capability Model and Context Packet Schema** spec was produced after architecture review.

The work in this session was documentation-focused rather than runtime-focused. No executable code was added. The primary outcome was a stronger spec that better aligns with the current foundation documents, especially around:

- mandatory deterministic-first processing
- bounded context requirements
- typed evidence and citation structures
- explicit ownership of `ImpactLevel` during Phase 0 and Phase 1

This note records the changes made after review and explains the intentional deviations from spec v2.

---

## Spec Gaps Resolved

### Gap 1 — Deterministic-first rule was weakened by a future exception path

Spec v2 allowed reasoning to proceed either with a valid context packet or through a future exception path approved by governance.

That language conflicted with the current foundation rules. `docs/ai-context.md` states that deterministic-first processing is mandatory and explicitly says models must not be called before deterministic preprocessing is complete. `docs/architecture.md` expresses the same rule at the system level.

Resolution:

- removed the future exception-path language from the spec
- made deterministic preprocessing mandatory in the spec itself
- explicitly stated that exceptions are out of scope for this spec

This brings the spec back into alignment with the current architecture posture.

### Gap 2 — `ImpactLevel` was required but ownership was unresolved

Spec v2 required `ImpactLevel` in both `ContextPacket` and `AssembleContextPacketRequest`, but left open who assigns that value.

That created an implementation gap because impact level is already a governance and routing primitive in the architecture.

Resolution:

- made `ImpactLevel` **caller-assigned** for Phase 0 and Phase 1
- deferred validation, recommendation, or override behavior to future governance and policy work

This gives runtime bootstrap a clear source of truth without pretending the policy engine already exists.

### Gap 3 — Evidence and citation structures were too loosely typed

Spec v2 used free-form string fields such as:

- `EvidenceItem.Type`
- `SourceReference.SourceType`
- `SourceReference.Location`

That would make future deterministic policy checks for evidence quality and citation precision harder to implement.

Resolution:

- introduced `EvidenceType`
- introduced `SourceReferenceType`
- introduced `SourceLocation`
- added `EvidenceContentMode`

This keeps the model light while making it much more usable for future validation and reviewer workflows.

### Gap 4 — Evidence payload shape risked violating bounded-context goals

Spec v2 required inline `Content` on every `EvidenceItem`.

That risked turning context packets into oversized transport objects instead of bounded reasoning artifacts.

Resolution:

- removed mandatory inline `Content`
- made `Summary` required
- introduced `ContentMode`
- made `ContentExcerpt` optional

This preserves the bounded-context goal while still allowing excerpts or inline payloads where justified.

### Gap 5 — A few high-value vocabulary fields needed to be tightened

Several fields in spec v2 remained stringly typed in ways that would likely create incompatible vocabularies during implementation.

Resolution:

- `RecordExecutionRequest.Outcome` now uses `ExecutionOutcome`
- `TokenBudget.Strategy` now uses `TokenBudgetStrategy`
- `ExtractionScope` remains a string for now, but the spec now explicitly says it should be narrowed after the first vertical slice stabilizes the vocabulary
- `Purpose` remains a string for Phase 0, but is now explicitly tracked as an open vocabulary question

This keeps Phase 0 practical without letting the first implementation invent unstable terminology by accident.

---

## Deviations from Spec v2

### Deviation 1 — Removed the deterministic-first exception path

Spec v2 allowed an exception path for reasoning before deterministic preprocessing was complete.

This was intentionally removed.

Reason for deviation:

The current foundation docs do not permit this flexibility. Keeping the exception would weaken a mandatory architecture rule and introduce a loophole before governance exists to control it.

### Deviation 2 — Strengthened evidence and source-reference typing

Spec v2 favored simpler string-based fields for evidence and citation metadata.

This was intentionally replaced with typed enums and a minimal location object.

Reason for deviation:

The architecture and AI context require enforceable evidence and citation discipline. The earlier model was too loose to support future deterministic policy checks reliably.

### Deviation 3 — Replaced mandatory inline evidence content with bounded content modes

Spec v2 required content on every evidence item.

This was intentionally changed to:

- required summary
- required source reference
- typed content mode
- optional excerpt

Reason for deviation:

The earlier structure encouraged packet bloat and cut against the project’s own bounded-context rule.

### Deviation 4 — Made `ImpactLevel` caller-assigned in early phases

Spec v2 left ownership unresolved.

This was intentionally resolved in the revised spec.

Reason for deviation:

Implementation bootstrap needs a concrete source of truth now. Caller-assigned impact levels are the simplest honest choice until governance and policy flows exist.

### Deviation 5 — Added a citation precision constraint

Spec v2 implied evidence discipline but did not make citation precision explicit in the constraints section.

This was intentionally added.

Reason for deviation:

Future reviewer and policy workflows will need structured source references and minimally precise locations to evaluate claims tied to code or documents.

---

## File-by-File Changes

### Modified files

| File | Change |
|---|---|
| `docs/specs/initial-capability-model/spec-v2.md` | Revised deterministic-first requirements, impact-level ownership, evidence model, citation model, and selected vocabularies to better align with core foundation docs |

### New files

| File | Purpose |
|---|---|
| `docs/specs/initial-capability-model/implementation-notes.md` | Records post-review changes, resolved gaps, and intentional deviations from spec v2 |

---

## Definition of Done — Status

- [x] Review feedback assessed against `docs/architecture.md`
- [x] Review feedback assessed against `docs/current-state.md`
- [x] Review feedback assessed against `docs/roadmap.md`
- [x] Review feedback assessed against `docs/ai-context.md`
- [x] Deterministic-first exception path removed
- [x] `ImpactLevel` ownership made explicit for Phase 0 and Phase 1
- [x] Evidence model tightened with typed enums and bounded content modes
- [x] Source reference model tightened with typed source kinds and minimal structured location metadata
- [x] Key stringly typed vocabulary fields reduced where needed for implementation readiness
- [x] Implementation notes created for this spec refinement session
- [ ] Runtime contracts scaffolded in `src/Specwright.Core/`
- [ ] First analyzer-oriented vertical slice implemented
- [ ] Policy model spec completed
- [ ] Execution journal persistence model specified

This session completed the documentation refinement needed to treat the revised spec as a stronger implementation baseline. It did **not** complete runtime implementation work.

---

## AI Reviewer System Prompt

No direct reviewer prompt change is required from this documentation-only refinement.

However, future reviewer guidance should enforce the following when checking workflow or capability specs:

- deterministic preprocessing must not be bypassed
- claims tied to code or documents must include structured evidence references
- context packets must remain bounded and should not become unreviewed payload dumps
- early-phase impact levels are caller-assigned unless a later governance spec says otherwise
