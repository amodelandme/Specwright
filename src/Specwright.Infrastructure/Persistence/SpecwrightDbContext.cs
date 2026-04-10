using Microsoft.EntityFrameworkCore;

namespace Specwright.Infrastructure.Persistence;

public sealed class SpecwrightDbContext(DbContextOptions<SpecwrightDbContext> options) : DbContext(options)
{
    public DbSet<CapabilityExecutionRecord> CapabilityExecutionRecords => Set<CapabilityExecutionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var capabilityExecution = modelBuilder.Entity<CapabilityExecutionRecord>();

        capabilityExecution.ToTable("capability_execution_records");
        capabilityExecution.HasKey(record => record.Id);

        capabilityExecution.Property(record => record.Id)
            .HasMaxLength(64);

        capabilityExecution.Property(record => record.WorkflowId)
            .HasMaxLength(256)
            .IsRequired();

        capabilityExecution.Property(record => record.CapabilityId)
            .HasMaxLength(256)
            .IsRequired();

        capabilityExecution.Property(record => record.Outcome)
            .HasMaxLength(64)
            .IsRequired();

        capabilityExecution.Property(record => record.EvidenceReferenceIdsJson)
            .HasColumnType("jsonb")
            .IsRequired();

        capabilityExecution.Property(record => record.ContextPacketId)
            .HasMaxLength(256);

        capabilityExecution.Property(record => record.RecordedAtUtc)
            .IsRequired();
    }
}
