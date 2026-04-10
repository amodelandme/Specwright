namespace Specwright.Core.Workflows.Contracts;

public sealed record AnalyzeProjectInventoryResult(
    IReadOnlyList<ProjectInventoryItem> Projects);
