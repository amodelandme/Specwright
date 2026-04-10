# AGENTS.md

## Purpose

This repository is for **Specwright**, a multi-agent, spec-driven engineering system for .NET backend development.

Specwright is designed to help teams build software with:
- persistent project memory
- structured specifications
- deterministic analysis
- bounded AI workflows
- policy and documentation enforcement

The agent should behave like a disciplined engineering assistant, not an autonomous code generator.

---

## Core Principles

- Specs are design documents, not tickets.
- Documentation is part of the system, not optional ceremony.
- Deterministic analysis should happen before model-driven reasoning.
- Changes must stay within the requested scope unless expansion is explicitly justified.
- The system should prefer evidence over confident prose.
- Architectural boundaries matter and should be protected.

---

## Required Context

Before starting significant work, read the relevant project documents.

### Always relevant
- `/docs/architecture.md`
- `/docs/current-state.md`
- `/docs/roadmap.md`
- `/docs/ai-context.md`

### For feature work
Also read:
- the relevant `spec.md`
- the relevant `implementation-notes.md`

Do not assume missing context. State what is known, unknown, and how to verify it.

---

## Expected Working Style

### 1. Start with analysis
Before making edits:
- summarize the task
- identify likely impacted files or projects
- identify risks or architectural boundaries
- propose a short plan for non-trivial work

### 2. Keep scope tight
Do not make repo-wide edits unless explicitly requested.

Prefer:
- small, reviewable changes
- clear project and folder boundaries
- explicit contracts and interfaces
- changes that are easy to verify with tests

### 3. Respect architecture
Avoid introducing:
- hidden coupling
- unnecessary abstractions
- speculative refactors
- broad changes outside the impact surface

Call out architecture tradeoffs before making large structural changes.

### 4. Prefer deterministic signals
When reasoning about code:
- inspect actual files
- use existing interfaces and types
- reference concrete symbols, projects, and paths
- avoid inventing behavior not supported by code or docs

### 5. Close the loop
After implementation:
- summarize changed files
- explain the architectural impact
- note any required doc updates
- identify follow-up work or open risks

---

## Rules for Editing

- Do not rename or reorganize large areas of the repository unless explicitly requested.
- Do not change unrelated files during a focused task.
- Do not silently alter public contracts without explaining why.
- Do not add new dependencies unless they serve a clear architectural purpose.
- Do not bypass tests, validation, or policy checks without stating that clearly.

---

## Review Mode Behavior

When asked to review code:
- cite exact files and symbols
- distinguish facts from inferences
- identify architecture rule violations clearly
- identify missing documentation or acceptance criteria coverage
- prefer concise, high-signal findings

---

## Implementation Bias

Prefer:
- explicit interfaces
- constructor injection
- modular services
- small DTOs and contracts
- testable units
- clear naming
- bounded workflows

Avoid:
- giant god services
- ambiguous abstractions
- deeply coupled orchestration
- unnecessary framework cleverness

---

## Specwright-Specific Guidance

This project should evolve toward:
- modular capabilities
- bounded context assembly
- deterministic-first workflows
- reviewer-driven enforcement
- codebase-aware brownfield analysis

Any implementation should support those directions rather than working against them.