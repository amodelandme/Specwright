namespace Specwright.Core.Workflows.Contracts;

public sealed record ProjectInventoryItem(
    string Name,
    string Path,
    string ProjectType,
    IReadOnlyList<string> TargetFrameworks,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> PackageReferences);
