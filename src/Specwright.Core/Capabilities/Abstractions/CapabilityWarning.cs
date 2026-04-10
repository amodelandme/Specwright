namespace Specwright.Core.Capabilities.Abstractions;

public sealed record CapabilityWarning(
    string Code,
    string Message,
    string? Target = null);
