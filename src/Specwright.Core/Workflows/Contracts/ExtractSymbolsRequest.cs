namespace Specwright.Core.Workflows.Contracts;

public sealed record ExtractSymbolsRequest(
    IReadOnlyList<string> TargetPaths,
    string ExtractionScope);
