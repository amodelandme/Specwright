namespace Specwright.Core.Capabilities.Abstractions;

public sealed record CapabilityResult<TResult>(
    ExecutionOutcome Outcome,
    TResult? Output,
    IReadOnlyList<CapabilityError> Errors,
    IReadOnlyList<CapabilityWarning> Warnings,
    IReadOnlyList<string> EvidenceReferenceIds,
    CapabilityExecutionMetadata Metadata)
    where TResult : class;
