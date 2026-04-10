# ADR-003: Deterministic-First Processing Rule

**Status:** Accepted  
**Date:** 2026-04-08

---

## Context

Specwright is being designed to support AI-assisted software workflows without collapsing into opaque prompt pipelines.

A core problem the platform is trying to solve is unbounded model behavior:

- reasoning without evidence
- weak traceability
- hidden assumptions
- inconsistent outputs
- inability to validate or audit workflow outcomes

Because the system is intended to support spec-driven engineering, bounded context assembly, policy enforcement, and future CI/PR validation, it needs a governing rule that keeps model use subordinate to evidence rather than the other way around.

The architecture already states the principle clearly:

> deterministic analysis before model reasoning

This decision needs to be formalized as an ADR because it is not merely an implementation preference. It is a load-bearing architectural rule that shapes capability design, workflow sequencing, policy checks, and future governance.

---

## Decision

Specwright will follow a **deterministic-first processing rule**.

This means:

- code, document, and diff facts must be extracted through deterministic capabilities before any model reasoning step
- model reasoning must operate on bounded evidence artifacts, such as context packets
- reasoning workflows must not begin from unconstrained repository or document input when deterministic extraction is possible
- claims affecting workflow outcomes should be traceable to supporting evidence

Examples of deterministic-first inputs include:

- repository structure analysis
- project inventory
- symbol extraction
- markdown fact extraction
- diff inspection
- metadata and lexical retrieval

This decision does not ban model reasoning. It constrains its place in the workflow.

Models are used after evidence assembly, not instead of it.

---

## Alternatives Considered

### 1. Model-first workflow design

This would allow the system to start with a model reading broad source material and producing judgments or summaries directly.

Rejected because:

- it weakens auditability
- it increases prompt and context sprawl
- it makes validation harder
- it encourages hidden reasoning over mutable inputs
- it conflicts with the platform’s policy-enforced ambitions

### 2. Fully manual evidence preparation outside the runtime

This would keep the runtime simpler by leaving evidence curation to humans or external tools.

Rejected because:

- it would undermine the goal of a real engineering system
- it would make workflow discipline too dependent on human memory and manual effort
- it would reduce the platform’s ability to enforce or validate its own process

### 3. No strong rule, only a recommendation

This would allow teams or features to choose deterministic-first behavior when convenient.

Rejected because:

- the architecture is explicitly trying to prevent drift into loose agent behavior
- a recommendation is too weak for future governance and policy validation
- the system needs a clear execution principle, not a preference

---

## Consequences

### Positive Consequences

- improves traceability and auditability
- creates a stable foundation for bounded context packets
- makes future policy checks more meaningful
- reduces model sprawl and unsupported claims
- keeps capabilities, workflows, and agents grounded in inspectable evidence
- better supports brownfield analysis where exact structural facts matter

### Negative Consequences

- adds upfront implementation effort because deterministic capabilities must exist before richer reasoning workflows
- may slow certain early experiments that could otherwise be prototyped with direct model prompts
- requires discipline to define what counts as adequate deterministic evidence for a workflow

### Ongoing Implications

Future specs and implementations should assume:

- deterministic capabilities come first in workflow design
- reasoning capabilities consume evidence, not raw uncontrolled context
- context packets or equivalent bounded artifacts are expected where model reasoning occurs
- unsupported architectural or workflow claims should be treated as lower-trust outputs
- policy and reviewer workflows should eventually validate evidence presence, not just output shape

---

## Exceptions and Boundaries

This rule is intended to govern normal platform behavior.

A future workflow may introduce a documented exception path where deterministic evidence is incomplete or unavailable, but such exceptions should be explicit, narrow, and reviewable.

Exceptions are not part of the default model.

---

## Review Triggers

This decision should be revisited if:

- deterministic preprocessing proves too expensive or too rigid for important real workflows
- certain categories of reasoning consistently require broader exploratory input before deterministic narrowing
- the cost of enforcing the rule materially outweighs the trust and validation benefits
- the platform develops a better evidence model that changes how “deterministic-first” should be interpreted

---

## Related Artifacts

- `docs/architecture.md`
- `docs/current-state.md`
- `docs/roadmap.md`
- `specwright-initial-capability-model-spec.md`
