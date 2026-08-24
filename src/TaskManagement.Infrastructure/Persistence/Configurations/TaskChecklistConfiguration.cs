using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskChecklistConfiguration : IEntityTypeConfiguration<TaskChecklist>
{
    public void Configure(EntityTypeBuilder<TaskChecklist> builder)
    {
        builder.ToTable("TaskChecklists");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.TaskId)
            .IsRequired();

        builder.Property(tc => tc.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(tc => tc.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tc => tc.Position)
            .IsRequired();

        builder.Property(tc => tc.CreatedAt)
            .IsRequired();

        builder.HasOne(tc => tc.Task)
            .WithMany(t => t.Checklists)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tc => new { tc.TaskId, tc.Position });
    }
}
