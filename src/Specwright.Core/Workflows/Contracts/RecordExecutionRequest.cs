using Specwright.Core.Capabilities.Abstractions;

namespace Specwright.Core.Workflows.Contracts;

public sealed record RecordExecutionRequest(
    string WorkflowId,
    string CapabilityId,
    ExecutionOutcome Outcome,
    IReadOnlyList<string> EvidenceReferenceIds,
    string? ContextPacketId);
