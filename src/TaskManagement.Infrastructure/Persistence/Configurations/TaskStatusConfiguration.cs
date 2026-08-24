using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatus>
{
    public void Configure(EntityTypeBuilder<TaskStatus> builder)
    {
        builder.ToTable("TaskStatuses");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.TenantId)
            .IsRequired();

        builder.Property(ts => ts.WorkspaceId)
            .IsRequired();

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ts => ts.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ts => ts.Color)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ts => ts.Position)
            .IsRequired();

        builder.Property(ts => ts.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(ts => ts.Workspace)
            .WithMany()
            .HasForeignKey(ts => ts.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ts => new { ts.WorkspaceId, ts.Name }).IsUnique();
        builder.HasIndex(ts => new { ts.WorkspaceId, ts.Position });
    }
}
