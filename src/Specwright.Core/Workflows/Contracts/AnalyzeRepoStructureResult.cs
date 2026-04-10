namespace Specwright.Core.Workflows.Contracts;

public sealed record AnalyzeRepoStructureResult(
    IReadOnlyList<string> Directories,
    IReadOnlyList<string> SolutionFiles,
    IReadOnlyList<string> ProjectFiles,
    IReadOnlyList<string> TopLevelFacts);
