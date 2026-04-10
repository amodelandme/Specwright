namespace Specwright.Core.Context;

public sealed record SourceReference(
    string SourceReferenceId,
    SourceReferenceType SourceType,
    string PathOrIdentifier,
    SourceLocation Location,
    DateTimeOffset ExtractedAtUtc);
