using Specwright.Core.Context;

namespace Specwright.Core.Workflows.Contracts;

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
