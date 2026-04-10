namespace Specwright.Core.Workflows.Contracts;

public sealed record AnalyzeProjectInventoryRequest(
    IReadOnlyList<string> SolutionOrProjectPaths);
