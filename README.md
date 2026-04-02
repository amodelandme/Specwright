# .NET Spec Kit

A spec-driven development workflow for .NET backend systems. I have been iterating through this idea for some time now. Figured I would throw into a repo.

---

## What Is This?

This kit gives you two things:

1. **Foundation documents** — living documents that serve as the source of truth for your project's architecture, current state, and roadmap. These live in `/docs` and are updated continuously throughout development.

2. **Spec-level templates** — documents that govern individual features from design through implementation. A spec defines what gets built and why. Implementation notes close the loop after the work is done. These live in `/templates`.

It also ships with two AI skill definitions — a **Senior .NET System Architect** and a **Senior .NET Backend Engineer** — that consume and produce these documents as part of a disciplined, spec-driven workflow.

---

## Folder Structure

```
/docs
  architecture.md          ← What the system is. Structural source of truth.
  current-state.md         ← Where things stand right now. Updated continuously.
  roadmap.md               ← Where things are going. Phase-gated plan.
  ai-context.md            ← Guardrails and conventions. Referenced by all three above.

/templates
  spec.md                  ← Design-time template. Produced by the Architect skill.
  implementation-notes.md  ← Post-implementation template. Produced by the Engineer skill.

/skills
  architect.md             ← AI skill: Senior .NET System Architect
  engineer.md              ← AI skill: Senior .NET Backend Engineer

/examples
  architecture.md          ← Real-world example of a filled architecture doc
  current-state.md         ← Real-world example of a filled current-state doc
  roadmap.md               ← Real-world example of a filled roadmap
  spec.md                  ← Real-world example of a filled feature spec
  implementation-notes.md  ← Real-world example of filled implementation notes
```

---

## The Workflow

```
┌─────────────────────────────────────────────────────────────────┐
│                        FOUNDATION LAYER                         │
│  architecture.md  ←→  current-state.md  ←→  roadmap.md         │
│                         ↑                                       │
│                    ai-context.md                                │
│              (single source of guardrails)                      │
└─────────────────────────────────────────────────────────────────┘
         ↑ read before designing          ↑ update after shipping
         │                                │
┌────────┴────────┐              ┌────────┴────────────┐
│  ARCHITECT      │              │  ENGINEER           │
│  skill          │              │  skill              │
│                 │              │                     │
│  Reads:         │              │  Reads:             │
│  - foundation   │   spec.md    │  - foundation docs  │
│  - requirements │  ─────────→  │  - spec.md          │
│                 │              │                     │
│  Produces:      │              │  Produces:          │
│  - spec.md      │              │  - working code     │
└─────────────────┘              │  - implementation-  │
                                 │    notes.md         │
                                 └─────────────────────┘
```

### Step by step

1. **Before any feature work**, the Architect reads the three foundation docs and the feature requirements.
2. **The Architect produces a `spec.md`** — an implementation-ready design document that defines what gets built, why specific decisions were made, and what done looks like.
3. **The Engineer reads the spec** critically — challenging gaps, validating assumptions, and raising concerns before writing a line of code.
4. **The Engineer implements** the feature according to the spec, deviating only when justified and documented.
5. **The Engineer produces `implementation-notes.md`** — a post-implementation record of what was built, gaps found in the spec, deviations made, and follow-up risks.
6. **The foundation docs are updated** to reflect the new state of the system.

---

## Getting Started

### For a new project

1. Copy `/docs/architecture.md`, `/docs/current-state.md`, `/docs/roadmap.md`, and `/docs/ai-context.md` into your project's `/docs` folder.
2. Fill them in. The templates contain instructive placeholder text — replace everything in `< >` brackets and follow the inline guidance.
3. Copy `/skills/architect.md` and `/skills/engineer.md` into your AI assistant's context (system prompt, project instructions, or equivalent).
4. When starting a feature, load the Architect skill and the three foundation docs. Ask it to produce a `spec.md`.
5. When implementing, load the Engineer skill and the relevant spec. Follow the workflow.

### For an existing project

Start with `current-state.md` — it's the fastest to fill in and immediately useful. Then backfill `architecture.md` with decisions that have already been made. `roadmap.md` last.

---

## Philosophy

- **Specs are not tickets.** A spec defines the design, the rationale, the constraints, and what done looks like. A ticket is a task. This kit produces specs.
- **Foundation docs are living.** They are wrong the moment they stop being updated. Treat them like code.
- **Decisions have rationale.** Every significant decision in a spec includes a "why." Future maintainers are not mind-readers.
- **The engineer is not a transcription service.** The Engineer skill is expected to push back on specs, catch gaps, and improve the design — not blindly execute.
- **Close the loop.** `implementation-notes.md` feeds back into the foundation docs. The cycle is: design → build → document → update → repeat.

---

## Compatibility

The skill definitions use plain markdown and are model-agnostic. They work with any AI assistant that accepts a system prompt or project-level instructions: Claude, GPT-4, Gemini, Copilot, or any local model.

---

## Examples

The `/examples` folder contains real-world filled documents from a production .NET feature flag service. Use them as reference when filling in your own templates.
