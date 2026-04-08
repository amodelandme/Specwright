# Specwright Architecture

## 1. Purpose

Specwright is a multi-agent, spec-driven engineering system for .NET backend development.

Its purpose is to improve how software is designed, implemented, reviewed, and documented when AI is part of the engineering process.

Specwright is not just a prompt workflow and not just a document kit. It is a structured engineering system built around:

- persistent project memory
- spec-driven execution
- bounded agent workflows
- architecture and documentation enforcement
- codebase awareness for greenfield and brownfield systems

The system is designed to help teams move from ad hoc AI usage toward disciplined, auditable, repeatable engineering workflows.

---

## 2. Architectural Goals

Specwright exists to achieve the following goals:

1. **Preserve project memory**
   - Maintain durable context across sessions, contributors, and AI runs.
   - Keep architecture, current state, roadmap, and rules visible and current.

2. **Make specifications first-class engineering artifacts**
   - Treat specs as design documents, not tickets.
   - Ensure implementation can be traced back to intentional design decisions.

3. **Enforce documentation and architectural discipline**
   - Require implementation notes and foundation updates as part of the delivery process.
   - Prevent documentation from becoming optional or stale.

4. **Bound AI behavior with deterministic controls**
   - Reduce hallucinations and drift through scoped context assembly, retrieval, policy checks, and evidence requirements.
   - Use models only where reasoning is needed, not where deterministic analysis is sufficient.

5. **Support brownfield understanding and onboarding**
   - Analyze existing codebases to generate architecture understanding, diagrams, and onboarding material.
   - Help teams adopt Specwright without needing a greenfield starting point.

6. **Create a modular system that can evolve**
   - Model agents as orchestrations of capabilities.
   - Allow new capabilities, policies, and integrations to be added without redesigning the entire platform.

---

## 3. Core System Model

Specwright is organized into three major conceptual layers.

### 3.1 Foundation Layer

The Foundation Layer acts as persistent system memory and shared engineering context.

It consists of:

- `architecture.md` → what the system is
- `current-state.md` → what currently exists
- `roadmap.md` → where the system is going
- `ai-context.md` → rules, constraints, and operating guidance

These documents serve as:

- shared memory for humans and AI
- architectural guardrails
- project-wide context for design and review workflows

### 3.2 Spec Layer

The Spec Layer governs feature-level design and implementation accountability.

It consists of:

- `spec.md` → design, decisions, constraints, acceptance criteria, impacted areas
- `implementation-notes.md` → reality after implementation, deviations, lessons, risks, and proposed foundation updates

This layer creates two forms of accountability:

- **design accountability** through structured specs
- **implementation accountability** through post-build documentation

### 3.3 Execution Layer

The Execution Layer is the operational system that coordinates analysis, reasoning, validation, and enforcement.

It includes:

- workflows
- agents
- capabilities
- context assembly
- retrieval
- policy enforcement
- CI and PR gates
- observability and execution records

This is the layer that turns Specwright from a documentation approach into an engineering system.

---

## 4. High-Level Architecture

At a high level, Specwright is a modular backend system with deterministic analysis at the center and LLM-powered reasoning at the edges.

### 4.1 Architectural Principle

The system follows this core rule:

**LLMs should not be the first component to inspect a codebase.**

Before a model is invoked, the system should first:

- inspect diffs
- extract code facts
- load rules and constraints
- retrieve relevant documents
- assemble a bounded context packet
- verify scope and token budget

Only then should an agent be asked to reason about the task.

### 4.2 Major Components

Specwright consists of the following major components:

1. **API / Entry Points**
   - Accept workflow requests from CLI, UI, editor integrations, or CI.

2. **Workflow Orchestrator**
   - Coordinates workflow execution.
   - Chooses which capabilities and agents run.
   - Enforces step ordering, escalation rules, and token budgets.

3. **Capability Host**
   - Registers atomic capabilities such as reading docs, analyzing diffs, validating rules, and generating diagrams.

4. **Context Assembler**
   - Builds the bounded context packet for a workflow.
   - Includes only the evidence and summaries needed for the task.

5. **Fact Extraction Services**
   - Deterministically inspect code, diffs, and documents.
   - Serve as trusted sources of technical facts.

6. **Retrieval Layer**
   - Retrieves relevant project memory, decisions, symbols, and other artifacts.
   - Uses metadata and lexical retrieval first.

7. **Policy Engine**
   - Enforces architecture rules, impact surface constraints, citation requirements, and artifact completeness.

8. **Agent Layer**
   - Executes reasoning workflows using bounded context.
   - Includes Reviewer, Architect, Engineer, Analyzer, and Foundation Generator.

9. **Execution Journal and Telemetry**
   - Captures run history, evidence, decisions, citations, and outcomes.

10. **CI / PR Enforcement Layer**
    - Applies gatekeeping behavior before code changes are approved.

---

## 5. Architectural Style

Specwright follows a modular service-oriented architecture within a .NET backend system.

### 5.1 Style Characteristics

- modular internal boundaries
- explicit contracts between services
- deterministic processing before probabilistic reasoning
- document-centric memory model
- workflow-driven orchestration
- policy-based enforcement
- incremental expansion toward multi-agent capabilities

### 5.2 Why This Style

This style supports the system’s needs better than a purely agent-centric architecture.

A purely agent-centric design would make behavior harder to predict, harder to audit, and harder to constrain. Specwright instead treats agents as one part of a larger system whose behavior is shaped by:

- structured inputs
- bounded scope
- explicit rules
- deterministic preprocessing
- execution records

This makes the platform more reliable for real engineering work.

---

## 6. Agent Model

Specwright is evolving toward a multi-agent system.

Agents are not the architecture itself. They are orchestrated reasoning units composed from lower-level capabilities.

### 6.1 Current and Emerging Agents

#### Architect

Responsibilities:
- design feature or system changes
- produce `spec.md`
- encode decisions, constraints, and acceptance criteria

#### Engineer

Responsibilities:
- implement against the spec
- challenge incorrect or incomplete design assumptions
- produce `implementation-notes.md`

#### Reviewer

Responsibilities:
- validate implementation against spec and architecture rules
- confirm documentation integrity
- act as a quality gate before PR approval

#### Analyzer

Responsibilities:
- inspect existing codebases
- generate architecture and current-state understanding
- support onboarding and system discovery
- produce diagrams and system summaries

#### Bootstrapper / Foundation Generator

Responsibilities:
- ask structured questions
- build initial foundation documents from answers
- accelerate onboarding for new or existing projects

### 6.2 Agent Design Principle

Agents should be composed from atomic capabilities rather than implemented as monolithic role blobs.

This allows:
- reuse across workflows
- clearer testing boundaries
- easier policy enforcement
- more flexible orchestration over time

---

## 7. Capability Model

Capabilities are the smallest useful units of system behavior.

Examples include:

- `read_docs`
- `generate_spec`
- `analyze_codebase`
- `validate_rules`
- `generate_diagrams`
- `update_foundation_docs`
- `diff_analysis`
- `retrieve_context`
- `bind_evidence`
- `summarize_scope`

### 7.1 Why Capabilities Matter

The capability model makes Specwright:

- modular
- composable
- extensible
- easier to test
- easier to reason about than a purely agent-driven design

Capabilities are the building blocks. Agents are orchestrations of those blocks.

---

## 8. Context and Memory Architecture

Context control is a first-class architectural concern.

The system should never rely on dumping large volumes of project state into a model prompt.

### 8.1 Memory Layers

Specwright uses four memory layers.

#### Permanent Memory
Stable project-wide truth:
- `architecture.md`
- `current-state.md`
- `roadmap.md`
- `ai-context.md`

#### Decision Memory
Feature and implementation history:
- `spec.md`
- `implementation-notes.md`

#### Working Memory
Task-local material:
- changed files
- changed symbols
- dependency slices
- relevant tests
- rule excerpts
- selected logs

#### Scratch Memory
Temporary, disposable intermediate material:
- summaries
- chunk notes
- transient drafts

Only the first three layers should persist across meaningful workflows.

### 8.2 Context Assembly Principles

The Context Assembler should:

- build workflow-specific context packets
- use metadata and lexical retrieval first
- include explicit scope boundaries
- include evidence and citations
- mark unknowns clearly
- enforce token budgets
- avoid repo-wide expansion unless escalation is justified

### 8.3 Context Packet Structure

A context packet should contain:

- workflow goal
- task scope
- relevant foundation summaries
- relevant spec and implementation-note summaries
- impacted files and symbols
- dependency slice
- evidence excerpts
- known unknowns
- applicable rules
- token budget metadata

This packet is the controlled interface between system knowledge and model reasoning.

---

## 9. Deterministic Analysis Layer

Specwright should prefer deterministic analysis wherever practical.

### 9.1 .NET Code Analysis

For .NET systems, Roslyn should be the primary code intelligence engine.

Roslyn is responsible for extracting facts such as:

- projects and references
- symbols and symbol relationships
- namespaces and type usage
- dependency flow
- changed symbols
- impacted layers

This makes the .NET path a native strength of the system.

### 9.2 Diff and Change Analysis

Git-based diff analysis should identify:

- changed files
- changed areas relative to the impact surface
- likely architectural boundaries touched
- review scope for the reviewer workflow

### 9.3 Document Parsing

Document parsing should normalize and index:

- foundation docs
- specs
- implementation notes
- rule definitions

This creates a searchable, structured memory layer for retrieval.

### 9.4 Future Polyglot Path

Although Specwright is .NET-first, the architecture should leave room for future polyglot analysis through sidecar parsers or language-specific analyzers.

---

## 10. Retrieval Architecture

Retrieval should be scoped, structured, and biased toward exactness.

### 10.1 Retrieval Order

The preferred retrieval order is:

1. metadata filter
2. exact symbol, file, or spec match
3. lexical retrieval
4. semantic retrieval if needed

### 10.2 Initial Storage Strategy

A lightweight initial design should use:

- markdown on disk as source of truth
- metadata persistence for indexing and workflow records
- lexical full-text retrieval for docs and structured artifacts

Semantic retrieval may be introduced later where it provides clear value, but it should not be the default mechanism for engineering truth.

---

## 11. Policy and Enforcement Architecture

Specwright is designed to move documentation and architecture compliance from optional behavior to enforced workflow.

### 11.1 Policy Types

Policies should cover:

- architecture dependency rules
- impact surface constraints
- required documentation artifacts
- acceptance criteria traceability
- citation and evidence requirements
- PR gate requirements

### 11.2 Enforcement Principle

The system should be able to reject a PR or workflow outcome when required artifacts or validations are missing.

Examples:
- no `implementation-notes.md` update
- no proposed foundation updates where required
- acceptance criteria not addressed
- architecture rule violations
- uncited claims in review findings

### 11.3 Reviewer as Quality Gate

The Reviewer agent is the central reasoning-based enforcement role, but its output should still be checked by deterministic policy validation where possible.

---

## 12. Brownfield Architecture Strategy

Brownfield onboarding is a major design concern.

Greenfield systems can adopt Specwright from the start. Existing systems require a path to discover and reconstruct architecture and current state.

### 12.1 Brownfield Goals

The Analyzer workflow should help answer:

- what does this system do
- how is it structured
- where are the major boundaries
- what are the important flows and dependencies
- what should a new engineer understand first

### 12.2 Brownfield Outputs

The analyzer should be able to generate:

- architecture draft
- current-state draft
- system diagrams
- dependency summaries
- onboarding material
- potential drift or hotspot findings

This makes Specwright relevant beyond greenfield process discipline.

---

## 13. CI / PR Workflow Architecture

Specwright should integrate with CI and PR workflows to enforce documentation and review discipline.

### 13.1 Target PR Flow

Target flow:

1. Engineer completes implementation
2. `implementation-notes.md` is updated
3. proposed foundation updates are prepared
4. Reviewer validates:
   - spec compliance
   - architecture rules
   - documentation integrity
5. CI gate approves or rejects PR readiness

### 13.2 Why This Matters

This is the transition point where Specwright becomes enforced rather than advisory.

It ensures:
- architectural drift is surfaced earlier
- documentation stays closer to reality
- design intent remains connected to implementation

---

## 14. Observability and Auditability

Specwright should be observable and auditable.

### 14.1 Observability Goals

The system should capture:

- workflow type
- context size and budget tier
- retrieved artifacts
- model usage
- citations and evidence
- policy outcomes
- final workflow result

### 14.2 Auditability Goals

A workflow outcome should be explainable after the fact.

It should be possible to answer:

- what context was used
- what evidence supported the conclusion
- what rules were applied
- what the model was asked to do
- why the system passed or failed the workflow

This is essential if Specwright is to be trusted in engineering environments.

---

## 15. Architectural Constraints

The following constraints shape the design.

1. **.NET-first core**
   - Core orchestration, analysis, policy, and workflow services should be implemented in .NET.

2. **Model-agnostic integration**
   - The system should not be hard-bound to a single model vendor.

3. **Deterministic-first processing**
   - Use deterministic computation before probabilistic reasoning.

4. **Markdown as source of truth**
   - Core engineering artifacts should remain human-readable and portable.

5. **Bounded context by default**
   - No workflow should assume unlimited context.

6. **Extensible capability system**
   - New capabilities and agents should be addable without major redesign.

7. **Policy-enforced discipline**
   - The architecture must support optional and enforced modes, with a path toward CI gating.

---

## 16. Planned Evolution

Specwright is expected to evolve in stages.

### 16.1 Near-Term

- Reviewer agent
- verifiable acceptance criteria in specs
- question-driven foundation generation
- prototype analyzer
- PR checklist and gates

### 16.2 Mid-Term

- richer codebase analysis
- diagram generation workflows
- stronger rule enforcement
- improved context scoring and retrieval
- more mature multi-agent orchestration

### 16.3 Long-Term

- codebase-aware engineering operating system for AI-assisted teams
- broader brownfield onboarding support
- deeper policy automation
- stronger knowledge graph behavior across project artifacts

---

## 17. Guiding Principles

The architecture of Specwright is guided by the following principles:

- design before implementation
- documentation as engineering truth, not ceremony
- bounded AI over unconstrained AI
- deterministic analysis before model reasoning
- modular capabilities over monolithic agents
- evidence over confident prose
- enforcement over optional discipline where it matters
- systems that learn through documented feedback loops

---

## 18. Summary

Specwright is a multi-agent, spec-driven engineering system with memory, enforcement, and codebase awareness.

Its architecture is designed to support disciplined software development when AI is part of the team.

It combines:

- persistent foundation documents
- feature-level specifications and implementation records
- deterministic analysis
- bounded context assembly
- modular capabilities
- agent-driven reasoning
- policy-based enforcement
- CI-integrated workflow controls

The result is a system intended not merely to assist software development, but to shape how software gets built in an AI-assisted engineering environment.

