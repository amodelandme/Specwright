namespace Specwright.Core.Workflows.Contracts;

public sealed record AnalyzeRepoStructureRequest(
    string RepositoryRoot,
    IReadOnlyList<string>? TargetPaths = null);
