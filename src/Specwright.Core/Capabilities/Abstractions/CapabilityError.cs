namespace Specwright.Core.Capabilities.Abstractions;

public sealed record CapabilityError(
    string Code,
    string Message,
    string? Target = null);
