namespace Specwright.Core.Context;

public sealed record ContextScope(
    string Repository,
    string? BranchOrRef,
    IReadOnlyList<string> TargetPaths,
    IReadOnlyList<string> IncludedArtifacts,
    IReadOnlyList<string> ExcludedArtifacts,
    string? ChangeSurface);
