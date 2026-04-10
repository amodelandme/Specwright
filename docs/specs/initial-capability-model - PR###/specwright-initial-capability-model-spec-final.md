# Spec: Initial Capability Model and Context Packet Schema

**Document:** `Docs/specs/initial-capability-model - PR###/specwright-initial-capability-model-spec-final.md`
**Status:** Draft  
**Branch:** `feature/initial-capability-model`
**Phase:** Phase 0 - Foundation Stabilization
**Author:** Jose / ChatGPT 5.4  
**Last Updated:** 2026-04-10

---

## Table of Contents
- [Summary](#summary)
- [Problem](#problem)
- [Goals](#goals)
- [Non-Goals](#non-goals)
- [Architectural Alignment](#architectural-alignment)
- [Core Concepts](#core-concepts)
- [Functional Requirements](#functional-requirements)
- [Proposed Initial Model](#proposed-initial-model)
- [Suggested Request and Response Contracts](#suggested-request-and-response-contracts)
- [Suggested Folder Placement](#suggested-folder-placement)
- [Initial Candidate Capabilities](#initial-candidate-capabilities)
- [Constraints](#constraints)
- [Impacted Areas](#impacted-areas)
- [Risks](#risks)
- [Acceptance Criteria](#acceptance-criteria)
- [Open Questions](#open-questions)
- [Recommended Follow-Up Specs](#recommended-follow-up-specs)
- [Decision Summary](#decision-summary)

---

## Summary

Define the first executable contract layer for Specwright by introducing:

1. an initial **capability model**
2. a **bounded context packet schema**

This spec exists to make the current architecture directly buildable.

It translates the architectural principles in `docs/architecture.md` into concrete runtime-facing contracts that can be used to scaffold the first implementation slice.

This spec does **not** introduce full workflow orchestration, full agent runtime behavior, or full governance enforcement. It defines the minimal contract shapes required to begin Phase 1 and support the first deterministic vertical slice in Phase 2.

---

## Problem

Specwright’s architecture is defined, but the runtime is not yet executable.

The largest gap between the current documented architecture and implementation is the lack of explicit contracts for:

- capabilities
- capability composition
- deterministic execution boundaries
- bounded context assembly
- evidence packaging for downstream reasoning or validation

Without these contracts:

- the runtime scaffold cannot be designed confidently
- the first analyzer workflow has no stable execution shape
- the agent model risks becoming vague or role-driven instead of capability-driven
- policy enforcement will have no standardized input artifact to validate

---

## Goals

### 3.1 Primary Goals

- define the first capability contract shape for the Specwright runtime
- define the first context packet schema for bounded evidence assembly
- preserve deterministic-first execution as a hard architectural rule
- support the first analyzer-oriented vertical slice
- provide enough structure to scaffold `Core`, `Api`, `Infrastructure`, and `Worker` boundaries without guessing

### 3.2 Secondary Goals

- make agent composition possible without introducing agent-specific runtime complexity yet
- make policy evaluation easier later by standardizing evidence and scope packaging
- keep the initial model small, explicit, and testable

---

## Non-Goals

This spec does **not**:

- define the complete workflow orchestrator
- define full agent behavior for Architect, Engineer, Reviewer, or Analyzer
- define the full policy engine implementation
- define semantic retrieval
- define full persistence schema for all execution records
- define UI or CLI interaction models
- define prompt content for model providers

---

## Architectural Alignment

This spec must align with the following rules from `docs/architecture.md`:

- deterministic-first processing
- bounded context by default
- markdown artifacts remain source of truth
- capabilities over role blobs
- policy-enforced discipline
- workflow integration over replacement

This spec must also support the `current-state.md` immediate next steps:

- define initial capability model
- define bounded context packet schema
- define initial policy model
- prepare for runtime bootstrap

---

## Core Concepts

### 6.1 Capability

A **capability** is an atomic, testable unit of system behavior that performs one bounded responsibility within a Specwright workflow.

A capability:

- has explicit inputs
- has explicit outputs
- declares whether it is deterministic or reasoning-based
- does not hide orchestration concerns inside itself
- can be composed into workflows and later into agents

Examples:

- `AnalyzeRepoStructureCapability`
- `AnalyzeProjectInventoryCapability`
- `ExtractSymbolsCapability`
- `AssembleContextPacketCapability`
- `RecordExecutionCapability`

### 6.2 Context Packet

A **context packet** is the bounded, structured artifact produced from deterministic analysis and supporting evidence.

It is the primary handoff object between:

- deterministic analysis steps
- reasoning steps
- reviewer or policy validation steps
- execution journaling

A context packet exists to prevent unbounded, opaque model invocation.

### 6.3 Deterministic CapabilityInitial Capability Model and Context Packet Schema
A capability is **reasoning-based** when it uses model inference or heuristic synthesis over bounded input.

Reasoning capabilities are allowed only after deterministic evidence is assembled.

---

## Functional Requirements

### 7.1 Capability Model Requirements

The system shall define an initial capability contract that includes:

- capability identifier
- capability category
- capability execution type
- input contract
- output contract
- failure contract
- optional evidence references
- optional execution metadata

The system shall support at least two execution types:

- `Deterministic`
- `Reasoning`

The system shall support capability composition into higher-level workflows without requiring agent-specific abstractions.

The initial capability model shall favor explicit contracts over convention-based or reflection-heavy discovery.

### 7.2 Context Packet Requirements

The system shall define a bounded context packet schema containing at minimum:

- packet identifier
- workflow identifier
- packet purpose
- scope definition
- evidence collection
- source references
- known unknowns
- constraints
- impact level
- token budget metadata
- packet creation metadata

The schema shall support citations or source references back to deterministic evidence.

The schema shall be serializable and persistable.

The schema shall support both human inspection and machine validation.

### 7.3 Deterministic-First Requirements

No reasoning capability shall execute without one of the following:

- a valid context packet
- an explicitly documented exception path approved by future governance rules

The initial implementation path shall assume that all analyzer-first workflows begin with deterministic capabilities.

---

## Proposed Initial Model

### 8.1 Capability Categories

Initial capability categories:

- `Analysis`
- `ContextAssembly`
- `Validation`
- `Persistence`
- `Reasoning`

These categories are descriptive and may later be expanded, but the initial model should remain small.

### 8.2 Suggested .NET Contract Types

Suggested foundational types in `Specwright.Core`:

- `ICapability<TRequest, TResult>`
- `CapabilityResult<TResult>`
- `CapabilityExecutionType`
- `CapabilityCategory`
- `ContextPacket`
- `ContextScope`
- `EvidenceItem`
- `SourceReference`
- `TokenBudget`
- `ContextPacketMetadata`
- `ImpactLevel`

These names are meant to reduce ambiguity during runtime bootstrap.

### 8.3 Suggested Interface Shape

```csharp
public interface ICapability<in TRequest, TResult>
    where TRequest : class
    where TResult : class
{
    string Id { get; }
    string Name { get; }
    CapabilityCategory Category { get; }
    CapabilityExecutionType ExecutionType { get; }

    Task<CapabilityResult<TResult>> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
```

### 8.4 Suggested Enums

```csharp
public enum CapabilityExecutionType
{
    Deterministic = 0,
    Reasoning = 1
}

public enum CapabilityCategory
{
    Analysis = 0,
    ContextAssembly = 1,
    Validation = 2,
    Persistence = 3,
    Reasoning = 4
}
```

### 8.5 Capability Result

All capabilities should return a structured result shape.

```csharp
public sealed record CapabilityResult<TResult>(
    bool Succeeded,
    TResult? Output,
    IReadOnlyList<CapabilityError> Errors,
    IReadOnlyList<CapabilityWarning> Warnings,
    IReadOnlyList<string> EvidenceReferenceIds,
    CapabilityExecutionMetadata Metadata)
    where TResult : class;
```

Where:

- `Output` is the typed result
- `Errors` is a structured collection, not a raw exception dump
- `Warnings` captures non-fatal concerns
- `EvidenceReferenceIds` points to supporting facts or discovered sources
- `Metadata` may include timing, version, execution source, or capability id

### 8.6 Structured Error and Warning Types

```csharp
public sealed record CapabilityError(
    string Code,
    string Message,
    string? Target = null);

public sealed record CapabilityWarning(
    string Code,
    string Message,
    string? Target = null);
```

### 8.7 Execution Metadata

```csharp
public sealed record CapabilityExecutionMetadata(
    string CapabilityId,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    TimeSpan Duration,
    string SchemaVersion);
```

### 8.8 Context Packet Schema

```csharp
public sealed record ContextPacket(
    string PacketId,
    string WorkflowId,
    string Purpose,
    ContextScope Scope,
    IReadOnlyList<EvidenceItem> Evidence,
    IReadOnlyList<SourceReference> SourceReferences,
    IReadOnlyList<string> Constraints,
    IReadOnlyList<string> KnownUnknowns,
    ImpactLevel ImpactLevel,
    TokenBudget TokenBudget,
    ContextPacketMetadata Metadata);
```

#### Field Details

##### PacketId

Unique identifier for the packet.

##### WorkflowId

Identifier linking the packet to a workflow execution.

##### Purpose

Describes why this packet exists.

Examples:

- analyzer summary
- reviewer evidence bundle
- implementation planning packet

##### Scope

```csharp
public sealed record ContextScope(
    string Repository,
    string? BranchOrRef,
    IReadOnlyList<string> TargetPaths,
    IReadOnlyList<string> IncludedArtifacts,
    IReadOnlyList<string> ExcludedArtifacts,
    string? ChangeSurface);
```

##### Evidence

```csharp
public sealed record EvidenceItem(
    string EvidenceId,
    string Type,
    string Summary,
    string Content,
    string SourceReferenceId,
    decimal? Confidence = null);
```

Confidence may initially be fixed or omitted for deterministic outputs, but the model should allow it.

##### Source References

```csharp
public sealed record SourceReference(
    string SourceReferenceId,
    string SourceType,
    string PathOrIdentifier,
    string? Location,
    DateTimeOffset ExtractedAtUtc);
```

Examples of source types:

- markdown
- code_symbol
- file
- diff
- project_manifest

##### Impact Level

The packet must carry:

- `L1`
- `L2`
- `L3`

Suggested shared enum:

```csharp
public enum ImpactLevel
{
    L1 = 1, // should we start at 0?
    L2 = 2, // should we start at 1?
    L3 = 3 // should we start at 2?
}
```

##### Token Budget

```csharp
public sealed record TokenBudget(
    int MaxInputTokens,
    int ReservedOutputTokens,
    string Strategy);
```

This allows future orchestration to enforce bounded reasoning.

##### Metadata

```csharp
public sealed record ContextPacketMetadata(
    DateTimeOffset CreatedAtUtc,
    string CreatedByCapability,
    string SchemaVersion);
```

---

## Suggested Request and Response Contracts

These contracts are intentionally narrow. They should live near their capability implementations or in a contracts folder within `Specwright.Core`.

### 9.1 Analyze Repo Structure

Suggested types:

- `AnalyzeRepoStructureRequest`
- `AnalyzeRepoStructureResult`

```csharp
public sealed record AnalyzeRepoStructureRequest(
    string RepositoryRoot,
    IReadOnlyList<string>? TargetPaths = null);

public sealed record AnalyzeRepoStructureResult(
    IReadOnlyList<string> Directories,
    IReadOnlyList<string> SolutionFiles,
    IReadOnlyList<string> ProjectFiles,
    IReadOnlyList<string> TopLevelFacts);
```

### 9.2 Analyze Project Inventory

Suggested types:

- `AnalyzeProjectInventoryRequest`
- `AnalyzeProjectInventoryResult`
- `ProjectInventoryItem`

```csharp
public sealed record AnalyzeProjectInventoryRequest(
    IReadOnlyList<string> SolutionOrProjectPaths);

public sealed record AnalyzeProjectInventoryResult(
    IReadOnlyList<ProjectInventoryItem> Projects);

public sealed record ProjectInventoryItem(
    string Name,
    string Path,
    string ProjectType,
    IReadOnlyList<string> TargetFrameworks,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> PackageReferences);
```

### 9.3 Extract Symbols

Suggested types:

- `ExtractSymbolsRequest`
- `ExtractSymbolsResult`
- `DiscoveredSymbol`

```csharp
public sealed record ExtractSymbolsRequest(
    IReadOnlyList<string> TargetPaths,
    string ExtractionScope);

public sealed record ExtractSymbolsResult(
    IReadOnlyList<DiscoveredSymbol> Symbols);

public sealed record DiscoveredSymbol(
    string Name,
    string Kind,
    string Namespace,
    string Path,
    string? Signature);
```

### 9.4 Assemble Context Packet

Suggested types:

- `AssembleContextPacketRequest`

```csharp
public sealed record AssembleContextPacketRequest(
    string WorkflowId,
    string Purpose,
    ContextScope Scope,
    IReadOnlyList<EvidenceItem> Evidence,
    IReadOnlyList<SourceReference> SourceReferences,
    IReadOnlyList<string> Constraints,
    IReadOnlyList<string> KnownUnknowns,
    ImpactLevel ImpactLevel,
    TokenBudget TokenBudget);
```

Output:

- `ContextPacket`

### 9.5 Record Execution

Suggested types:

- `RecordExecutionRequest`
- `RecordExecutionResult`

```csharp
public sealed record RecordExecutionRequest(
    string WorkflowId,
    string CapabilityId,
    string Outcome,
    IReadOnlyList<string> EvidenceReferenceIds,
    string? ContextPacketId);

public sealed record RecordExecutionResult(
    string ExecutionRecordId);
```

---

## Suggested Folder Placement

Initial placement inside `src/Specwright.Core/`:

```text
Specwright.Core/
  Capabilities/
    Abstractions/
      ICapability.cs
      CapabilityResult.cs
      CapabilityExecutionType.cs
      CapabilityCategory.cs
      CapabilityError.cs
      CapabilityWarning.cs
      CapabilityExecutionMetadata.cs
  Context/
    ContextPacket.cs
    ContextScope.cs
    EvidenceItem.cs
    SourceReference.cs
    TokenBudget.cs
    ContextPacketMetadata.cs
    ImpactLevel.cs
  Workflows/
    Contracts/
      AnalyzeRepoStructureRequest.cs
      AnalyzeRepoStructureResult.cs
      AnalyzeProjectInventoryRequest.cs
      AnalyzeProjectInventoryResult.cs
      ProjectInventoryItem.cs
      ExtractSymbolsRequest.cs
      ExtractSymbolsResult.cs
      DiscoveredSymbol.cs
      AssembleContextPacketRequest.cs
      RecordExecutionRequest.cs
      RecordExecutionResult.cs
```

This layout is only an initial recommendation.

The key intent is:

- capability abstractions stay separate from workflow-specific contracts
- context artifacts remain first-class
- contracts are easy to test and version

---

## Initial Candidate Capabilities

The first vertical slice should support the following capabilities:

### 11.1 `AnalyzeRepoStructureCapability`

Determines high-level repository shape.

Inputs:

- repository root or target path set

Outputs:

- directory inventory
- recognized solution and project files
- high-level project layout facts

Execution Type:

- deterministic

### 11.2 `AnalyzeProjectInventoryCapability`

Determines project-level inventory for .NET solutions.

Inputs:

- solution or project files

Outputs:

- project names
- project types
- target frameworks
- project references
- package references

Execution Type:

- deterministic

### 11.3 `ExtractSymbolsCapability`

Extracts selected code symbols using deterministic analysis.

Inputs:

- target files or projects
- extraction scope

Outputs:

- discovered types
- methods
- interfaces
- namespaces
- references

Execution Type:

- deterministic

### 11.4 `AssembleContextPacketCapability`

Builds a bounded packet from deterministic outputs.

Inputs:

- workflow purpose
- scope
- evidence inputs
- constraints
- impact level

Outputs:

- `ContextPacket`

Execution Type:

- deterministic

### 11.5 `RecordExecutionCapability`

Persists execution results and references.

Inputs:

- workflow metadata
- capability results
- context packet references

Outputs:

- persisted execution record reference

Execution Type:

- deterministic

---

## Constraints

### 12.1 Deterministic-First Constraint

Reasoning capabilities must not be the first capability in an analyzer-oriented workflow.

### 12.2 Explicit Contracts Constraint

Capabilities must use typed request and response contracts.

### 12.3 No Hidden Orchestration Constraint

Capabilities must not internally decide workflow routing or policy outcomes.

### 12.4 No Raw Exception Leakage Constraint

Capability results must expose structured errors suitable for workflow handling.

### 12.5 Bounded Context Constraint

Any reasoning input must come from a context packet or equivalent bounded evidence artifact.

### 12.6 Serialization Constraint

All initial contract types should be safe to serialize to JSON without custom converter complexity unless a later need proves otherwise.

### 12.7 Versioning Constraint

`CapabilityExecutionMetadata` and `ContextPacketMetadata` must carry a schema version to support contract evolution.

---

## Impacted Areas

This spec is expected to influence:

- `docs/architecture.md`
  - if contract terms need refinement
- `docs/current-state.md`
  - once contracts are actually documented or scaffolded
- `docs/roadmap.md`
  - if sequencing or deliverables are refined
- `docs/specs/`
  - as the home for this and future feature or workflow specs
- `src/Specwright.Core/`
- `src/Specwright.Api/`
- `src/Specwright.Infrastructure/`
- `src/Specwright.Worker/`
- `tests/Specwright.UnitTests/`
- `tests/Specwright.IntegrationTests/`

No runtime implementation is required by this spec alone, but it is intended to directly enable the bootstrap and first vertical slice.

---

## Risks

### 14.1 Over-Abstracting Too Early

If the capability model becomes too generic, it may slow down the first working slice.

### 14.2 Schema Bloat

If the context packet tries to solve future needs too early, it may become hard to implement or validate.

### 14.3 Weak Evidence Standards

If evidence references are optional in practice, reasoning workflows may drift into opaque behavior.

### 14.4 Role Leakage

If agent concerns are mixed into capability contracts too early, the capability-first design will erode.

### 14.5 Premature Infrastructure Coupling

If persistence or provider-specific concerns leak into `Specwright.Core`, future evolution will get sticky fast.

---

## Acceptance Criteria

### Capability Model

- [ ] An initial capability contract is documented clearly enough to implement in code
- [ ] Capability categories are defined
- [ ] Deterministic vs reasoning execution types are defined
- [ ] Capability result structure is defined
- [ ] Initial candidate capabilities are named and scoped

### Context Packet

- [ ] A context packet schema is documented
- [ ] The schema includes scope, evidence, source references, constraints, and known unknowns
- [ ] The schema supports carrying an impact level
- [ ] The schema supports token budget metadata
- [ ] The schema is suitable for persistence and machine validation

### Architectural Compliance

- [ ] The model preserves deterministic-first processing
- [ ] The model supports bounded context by default
- [ ] The model does not require agent-specific runtime behavior
- [ ] The model is narrow enough to support the first vertical slice without excessive abstraction

### Execution Readiness

- [ ] This spec gives enough clarity to scaffold the runtime projects
- [ ] This spec gives enough clarity to begin implementation of the first analyzer-oriented workflow
- [ ] Suggested .NET contract names are explicit enough to reduce scaffold-time guesswork
- [ ] Suggested folder placement is clear enough to establish initial project structure

---

## Open Questions

### 16.1 Capability Registration

Should capability registration be explicit in DI only, or should the system also support metadata-driven discovery?

### 16.2 Packet Persistence Shape

Should the full packet be stored as JSON initially, or should evidence and source references be partially normalized from the start?

### 16.3 Impact Level Assignment

Should `ImpactLevel` be assigned by the caller, by deterministic heuristics, or by later governance logic?

### 16.4 Error Standardization

Should capability errors align to a single shared problem-details-style envelope across the runtime?

### 16.5 Evidence Content Size

Should evidence items store full extracted content, summaries, or references-only by default?

### 16.6 Result Object Standardization

Should `CapabilityResult<TResult>` remain the base result abstraction everywhere, or should workflow-level result shapes wrap it?

---

## Recommended Follow-Up Specs

After this spec is accepted, the next likely specs are:

1. **Initial Policy Model**
2. **Runtime Bootstrap and Project Boundary Rules**
3. **First Analyzer Workflow Vertical Slice**
4. **Execution Journal Persistence Model**

---

## Decision Summary

Specwright will introduce an initial contract-first runtime foundation by defining:

- a small, explicit capability model
- a bounded context packet schema
- deterministic-first execution constraints
- suggested .NET contract types and folder placement
- initial candidate capabilities for the first analyzer slice

This spec is the bridge from architecture definition to runtime bootstrap.
