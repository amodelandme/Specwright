# Specwright

> **🚧 Work in Progress** — This framework is being built in the open, alongside the [FeatureFlagService](https://github.com/amodelandme/FeatureFlagService) reference implementation that proves it works. Both projects evolve together.

```
A model-agnostic, spec-driven AI development workflow for .NET backend systems.
```

---

## The Problem

Every time you open a new AI chat session, your assistant has no idea what you're building, what decisions you've already made, or what rules your codebase lives by. You re-explain the architecture. You remind it about the DTO boundary. You watch it violate the same layer rule it violated last Tuesday.

Most AI workflow setups bolt context onto the problem after the fact — a `CLAUDE.md` here, a `.cursor/rules` there. Specwright inverts that. **The foundation documents _are_ the workflow.** The AI skills read them. The specs enforce them. The implementation notes close the loop.

---

## What It Is

Specwright gives you two things that work together:

**1. Foundation documents** — four living markdown files that serve as the single source of truth for your project's architecture, current state, roadmap, and conventions. Updated continuously. Referenced by every AI session.

**2. AI skill definitions** — an Architect skill that designs features and produces specs, and an Engineer skill that implements them critically — challenging gaps, enforcing layer boundaries, and closing the loop with implementation notes.

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
│  Reads:         │   spec.md    │  Reads:             │
│  - foundation   │  ─────────→  │  - foundation docs  │
│  - requirements │              │  - spec.md          │
│                 │              │                     │
│  Produces:      │              │  Produces:          │
│  - spec.md      │              │  - working code     │
└─────────────────┘              │  - implementation-  │
                                 │    notes.md         │
                                 └─────────────────────┘
```

### Step by step

1. **Before any feature work**, the Architect reads the four foundation docs and the feature requirements.
2. **The Architect produces a `spec.md`** — an implementation-ready design document that defines what gets built, why specific decisions were made, and what done looks like.
3. **The Engineer reads the spec critically** — challenging gaps, validating assumptions, and raising concerns before writing a line of code.
4. **The Engineer implements** the feature according to the spec, deviating only when justified and documented.
5. **The Engineer produces `implementation-notes.md`** — a post-implementation record of what was built, gaps found in the spec, deviations made, and follow-up risks.
6. **The foundation docs are updated** to reflect the new state of the system.

---

## Folder Structure

```
specwright/
├── docs/
│   ├── architecture.md          ← what the system is
│   ├── current-state.md         ← where things stand right now
│   ├── roadmap.md               ← where things are going
│   └── ai-context.md            ← guardrails referenced by all three
│
├── skills/
│   ├── architect.md             ← senior .NET system architect
│   └── engineer.md              ← senior .NET backend engineer
│
├── templates/
│   ├── foundation/              ← doc templates for new projects
│   ├── feature/                 ← spec.md + implementation-notes.md
│   └── developer/               ← developer-context-template.md
│
├── examples/                    ← real filled docs from FeatureFlagService
│   ├── architecture.md
│   ├── current-state.md
│   ├── roadmap.md
│   ├── spec.md
│   └── implementation-notes.md
│
├── developer-context.md         ← personal config — gitignored, never committed
├── .gitignore
└── README.md
```

---

## The Skills

Both skills load your foundation documents and `developer-context.md` at the start of every session. They adapt explanation depth, feedback tone, diagram usage, and mentorship behavior to match the profile you declare there.

### `skills/architect.md` — Senior .NET System Architect

Reads the foundation docs, designs the feature, and produces an implementation-ready `spec.md`. Every decision includes a rationale. Every scope item names a file and a layer.

**Reads on load:**
- `docs/architecture.md`
- `docs/current-state.md`
- `docs/roadmap.md`
- `docs/ai-context.md`
- `developer-context.md`

### `skills/engineer.md` — Senior .NET Backend Engineer

Reads the spec critically — challenges gaps, validates assumptions — then implements, verifies, and produces `implementation-notes.md` to close the loop. Pushes back when the spec introduces unnecessary complexity or violates layer boundaries.

**Reads on load:**
- `docs/architecture.md`
- `docs/current-state.md`
- `docs/ai-context.md`
- `docs/decisions/<feature>/spec.md`
- `developer-context.md`

---

## Developer Context

`developer-context.md` is a personal configuration file that tells the skills who is doing the building — not what is being built. It captures your current level, learning style, career goals, engineering values, and interaction preferences.

It is **gitignored by design**. The project's source of truth is shared. Your personal AI configuration is not.

Copy `templates/developer/developer-context-template.md` to your repo root, fill it in, and add `developer-context.md` to your `.gitignore`. Both skills will adapt to it automatically.

---

## Getting Started

### New project

1. Copy the four files from `templates/foundation` into your project's `/docs` folder. Replace every `< >` placeholder with your project's reality.
2. Copy `templates/developer/developer-context-template.md` to your repo root as `developer-context.md`. Fill it in honestly. Add it to `.gitignore`.
3. Load `skills/architect.md` into your AI assistant's system prompt or project instructions alongside the four foundation docs. Ask it to design your first feature.
4. Switch to `skills/engineer.md` with the generated `spec.md` and the foundation docs. The Engineer challenges the spec, implements, and closes the loop.

### Existing project

Start with `docs/current-state.md` — it's the fastest to fill in and immediately useful. Backfill `docs/architecture.md` with decisions already made. `docs/roadmap.md` last.

---

## Compatibility

The skill definitions use plain markdown and are **model-agnostic**. They work with any AI assistant that accepts a system prompt or project-level instructions:

`Claude` · `GPT-4` · `Gemini` · `Copilot` · `Continue` · `local models`

---

## Project Status

| Component | Status | Notes |
|---|---|---|
| Foundation doc templates | ✅ Complete | All four with inline guidance |
| Architect skill | ✅ Complete | Adaptive via `developer-context.md` |
| Engineer skill | ✅ Complete | Adaptive via `developer-context.md` |
| Spec + impl-notes templates | ✅ Complete | In `templates/feature/` |
| Developer context template | ✅ Complete | Personal config, gitignored |
| FeatureFlagService examples | ✅ Complete | Real filled docs in `examples/` |
| Setup automation / CLI | 🔄 In Progress | One-command project initialization |
| Multi-skill session handoff | ⏳ Planned | Handoff protocols between sessions |
| VS Code extension | ⏳ Planned | Context loading without copy-paste |

---

## Philosophy

**Specs are not tickets.** A spec defines the design, the rationale, the constraints, and what done looks like. A ticket is a task. This framework produces specs.

**Foundation docs are living.** They are wrong the moment they stop being updated. Treat them like code — version them, maintain them, trust them.

**Decisions have rationale.** Every significant choice in a spec includes a "why." Future maintainers are not mind-readers. Neither are AI assistants.

**The engineer is not a transcription service.** The Engineer skill is expected to push back on specs, catch gaps, and improve the design — not blindly execute.

**Context is personal.** `developer-context.md` is gitignored by design. The project's source of truth is shared. The developer's personal AI configuration is not.

**Model-agnostic by design.** Plain markdown, no vendor lock-in. Works with any AI assistant that accepts a system prompt.

---

## Examples

The `/examples` folder contains real filled documents from a production .NET feature flag service. Use them as reference when filling in your own templates — they show what the framework looks like in practice, not just in theory.

---

## License

MIT — Jose Rodriguez-Marrero

---

> **Contributing:** This project is in active development. Ideas, issues, and pull requests are welcome. If you're using Specwright on your own project, open an issue and share what's working and what isn't.
