using Microsoft.EntityFrameworkCore;
using Specwright.Core.Capabilities.Abstractions;
using Specwright.Core.Workflows.Contracts;
using Specwright.Infrastructure.Capabilities;
using Specwright.Infrastructure.Persistence;

namespace Specwright.UnitTests;

public class RecordExecutionCapabilityTests
{
    [Fact]
    public async Task ExecuteAsync_PersistsExecutionRecordAndReturnsResult()
    {
        var options = new DbContextOptionsBuilder<SpecwrightDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        await using var dbContext = new SpecwrightDbContext(options);
        var capability = new RecordExecutionCapability(dbContext, TimeProvider.System);

        var request = new RecordExecutionRequest(
            WorkflowId: "workflow-1",
            CapabilityId: "record_execution",
            Outcome: ExecutionOutcome.PartiallySucceeded,
            EvidenceReferenceIds: ["evidence-1", "evidence-2"],
            ContextPacketId: "packet-1");

        var result = await capability.ExecuteAsync(request);

        Assert.Equal(ExecutionOutcome.Succeeded, result.Outcome);
        Assert.NotNull(result.Output);
        Assert.Equal("record_execution", result.Metadata.CapabilityId);

        var persistedRecord = await dbContext.CapabilityExecutionRecords.SingleAsync();

        Assert.Equal("workflow-1", persistedRecord.WorkflowId);
        Assert.Equal("record_execution", persistedRecord.CapabilityId);
        Assert.Equal("PartiallySucceeded", persistedRecord.Outcome);
        Assert.Equal("packet-1", persistedRecord.ContextPacketId);
    }
}
