# AI Context - Specwright

**Last updated:** 2026-04-08

> This is the single source of truth for engineering guardrails and architecture constraints.
> Read this file before producing designs, code changes, reviews, or roadmap updates.

---

## Architecture Rules

- **Deterministic-first processing is mandatory.** Run code/doc/diff analysis before model reasoning.
- **Bounded context is required.** No task should assume unlimited context windows.
- **Markdown artifacts are source of truth.** Foundation and spec artifacts must remain human-readable and versioned.
- **Capabilities are atomic and composable.** Avoid monolithic agent logic that mixes unrelated concerns.
- **Policies are enforceable, not advisory-only.** Missing required artifacts or uncited claims are failures.

---

## Initial Module Boundaries

```text
Api             -> Core
Worker          -> Core
Infrastructure  -> Core
Core            -> (no infrastructure or host references)
AppHost         -> references runnable projects only
```

### Ownership

- `Api`: workflow entry points and request validation
- `Core`: orchestration, capabilities, context assembly, policy evaluation
- `Infrastructure`: PostgreSQL, adapters, provider integrations
- `Worker`: asynchronous or batch workflow execution

---

## Conventions and Patterns

### Solution and Naming

- Project naming uses `Specwright.<ModuleName>`.
- Solution lives under `src/` with test projects under `tests/`.
- Foundation docs always live in `docs/`.

### Data and Persistence

- PostgreSQL is the primary system store.
- EF Core 10 + Npgsql are the default persistence stack.
- Schema changes are tracked with migrations.

### Workflow Design

- Each workflow defines: goal, scope, required evidence, output contract, and policy checks.
- Unknowns must be explicit in outputs.
- Claims tied to code/docs must include evidence references.

---

## What Not To Do

- Do not call models before deterministic preprocessing is complete.
- Do not merge retrieval, policy, and orchestration responsibilities into one component.
- Do not rely on semantic retrieval as default when exact/lexical retrieval is sufficient.
- Do not introduce provider lock-in in core contracts.
- Do not ship workflow outputs without evidence/citation metadata where required.

---

## Testing Conventions

- Unit tests cover capability behavior and policy logic.
- Integration tests cover end-to-end workflows and persistence.
- Every new workflow requires at least one end-to-end happy-path integration test.
- Policy failure paths are tested, not assumed.

---

## Current Constraints

- Project is in bootstrap phase; prioritize foundation stability over breadth.
- Initial architecture is modular monolith with explicit boundaries.
- Multi-service extraction happens only after boundary pressure is demonstrated.

---

## Before You Change Anything

1. Read `docs/architecture.md` for system intent and boundaries.
2. Read `docs/current-state.md` for actual implementation status.
3. Read `docs/roadmap.md` for phase priorities and dependencies.
4. Confirm the change aligns with current phase goals.
5. Update foundation docs when implementation reality changes.

---

## Session Start Checklist

- Read `docs/ai-context.md`
- Read `docs/architecture.md`
- Read `docs/current-state.md`
- Read `docs/roadmap.md`
- Confirm active phase before planning or coding
