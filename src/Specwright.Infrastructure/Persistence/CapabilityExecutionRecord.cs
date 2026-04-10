namespace Specwright.Infrastructure.Persistence;

public sealed class CapabilityExecutionRecord
{
    public string Id { get; set; } = string.Empty;

    public string WorkflowId { get; set; } = string.Empty;

    public string CapabilityId { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;

    public string EvidenceReferenceIdsJson { get; set; } = "[]";

    public string? ContextPacketId { get; set; }

    public DateTimeOffset RecordedAtUtc { get; set; }
}
