namespace Specwright.Core.Context;

public sealed record SourceLocation(
    string Kind,
    string? Start,
    string? End,
    string? Label);
