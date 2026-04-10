namespace Specwright.Core.Workflows.Contracts;

public sealed record ExtractSymbolsResult(
    IReadOnlyList<DiscoveredSymbol> Symbols);
