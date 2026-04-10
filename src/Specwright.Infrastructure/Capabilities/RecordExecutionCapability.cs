using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Specwright.Core.Capabilities.Abstractions;
using Specwright.Core.Workflows.Contracts;
using Specwright.Infrastructure.Persistence;

namespace Specwright.Infrastructure.Capabilities;

public sealed class RecordExecutionCapability(
    SpecwrightDbContext dbContext,
    TimeProvider timeProvider) : IRecordExecutionCapability
{
    public const string CapabilityIdValue = "record_execution";
    private const string SchemaVersion = "1.0";

    public string Id => CapabilityIdValue;

    public string Name => "Record Execution";

    public CapabilityCategory Category => CapabilityCategory.Persistence;

    public CapabilityExecutionType ExecutionType => CapabilityExecutionType.Deterministic;

    public async Task<CapabilityResult<RecordExecutionResult>> ExecuteAsync(
        RecordExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAtUtc = timeProvider.GetUtcNow();

        try
        {
            var record = new CapabilityExecutionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                WorkflowId = request.WorkflowId,
                CapabilityId = request.CapabilityId,
                Outcome = request.Outcome.ToString(),
                EvidenceReferenceIdsJson = JsonSerializer.Serialize(request.EvidenceReferenceIds),
                ContextPacketId = request.ContextPacketId,
                RecordedAtUtc = startedAtUtc
            };

            dbContext.CapabilityExecutionRecords.Add(record);
            await dbContext.SaveChangesAsync(cancellationToken);

            var completedAtUtc = timeProvider.GetUtcNow();

            return new CapabilityResult<RecordExecutionResult>(
                Outcome: ExecutionOutcome.Succeeded,
                Output: new RecordExecutionResult(record.Id),
                Errors: Array.Empty<CapabilityError>(),
                Warnings: Array.Empty<CapabilityWarning>(),
                EvidenceReferenceIds: request.EvidenceReferenceIds,
                Metadata: new CapabilityExecutionMetadata(
                    CapabilityId: Id,
                    StartedAtUtc: startedAtUtc,
                    CompletedAtUtc: completedAtUtc,
                    Duration: completedAtUtc - startedAtUtc,
                    SchemaVersion: SchemaVersion));
        }
        catch (DbUpdateException)
        {
            var completedAtUtc = timeProvider.GetUtcNow();

            return new CapabilityResult<RecordExecutionResult>(
                Outcome: ExecutionOutcome.Failed,
                Output: null,
                Errors:
                [
                    new CapabilityError(
                        Code: "execution_persistence_failed",
                        Message: "Failed to persist the execution record.",
                        Target: nameof(RecordExecutionRequest))
                ],
                Warnings: Array.Empty<CapabilityWarning>(),
                EvidenceReferenceIds: request.EvidenceReferenceIds,
                Metadata: new CapabilityExecutionMetadata(
                    CapabilityId: Id,
                    StartedAtUtc: startedAtUtc,
                    CompletedAtUtc: completedAtUtc,
                    Duration: completedAtUtc - startedAtUtc,
                    SchemaVersion: SchemaVersion));
        }
        catch (Exception)
        {
            var completedAtUtc = timeProvider.GetUtcNow();

            return new CapabilityResult<RecordExecutionResult>(
                Outcome: ExecutionOutcome.Failed,
                Output: null,
                Errors:
                [
                    new CapabilityError(
                        Code: "unexpected_execution_persistence_error",
                        Message: "An unexpected error occurred while recording the execution result.",
                        Target: nameof(RecordExecutionRequest))
                ],
                Warnings: Array.Empty<CapabilityWarning>(),
                EvidenceReferenceIds: request.EvidenceReferenceIds,
                Metadata: new CapabilityExecutionMetadata(
                    CapabilityId: Id,
                    StartedAtUtc: startedAtUtc,
                    CompletedAtUtc: completedAtUtc,
                    Duration: completedAtUtc - startedAtUtc,
                    SchemaVersion: SchemaVersion));
        }
    }
}
