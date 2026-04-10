namespace Specwright.Core.Capabilities.Abstractions;

public sealed record CapabilityExecutionMetadata(
    string CapabilityId,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    TimeSpan Duration,
    string SchemaVersion);
