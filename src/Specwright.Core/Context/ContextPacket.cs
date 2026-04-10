namespace Specwright.Core.Context;

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
