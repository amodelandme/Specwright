namespace Specwright.Core.Context;

public sealed record EvidenceItem(
    string EvidenceId,
    EvidenceType Type,
    string Summary,
    string SourceReferenceId,
    EvidenceContentMode ContentMode,
    string? ContentExcerpt = null,
    decimal? Confidence = null);
