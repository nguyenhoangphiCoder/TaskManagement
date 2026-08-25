using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TenantId)
            .IsRequired();

        builder.Property(t => t.WorkspaceId)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.Property(t => t.ParentTaskId)
            .IsRequired(false);

        builder.Property(t => t.Code)
            .HasConversion(
                c => c.Value,
                v => TaskCode.FromString(v))
            .HasColumnName("Code")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.Code);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(5000);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.StatusId)
            .IsRequired();

        builder.Property(t => t.StartDate)
            .IsRequired(false);

        builder.Property(t => t.DueDate)
            .IsRequired(false);

        builder.Property(t => t.EstimatedMinutes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.ActualMinutes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.SortOrder)
            .IsRequired();

        builder.Property(t => t.SubtaskLevel)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Workspace)
            .WithMany()
            .HasForeignKey(t => t.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Status)
            .WithMany()
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Members)
            .WithOne(tm => tm.Task)
            .HasForeignKey(tm => tm.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Dependencies)
            .WithOne(td => td.Task)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Checklists)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Comments)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Attachments)
            .WithOne(ta => ta.Task)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TimeEntries)
            .WithOne(te => te.Task)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.StatusId);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.DueDate);
        builder.HasIndex(t => t.ParentTaskId);
        builder.HasIndex(t => new { t.ProjectId, t.SortOrder });
    }
}
