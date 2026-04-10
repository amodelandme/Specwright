namespace Specwright.Core.Context;

public sealed record ContextPacketMetadata(
    DateTimeOffset CreatedAtUtc,
    string CreatedByCapability,
    string SchemaVersion);
