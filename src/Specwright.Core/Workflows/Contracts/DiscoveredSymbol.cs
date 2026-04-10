namespace Specwright.Core.Workflows.Contracts;

public sealed record DiscoveredSymbol(
    string Name,
    string Kind,
    string Namespace,
    string Path,
    string? Signature);
