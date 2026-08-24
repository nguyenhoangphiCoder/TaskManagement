using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");

        builder.HasKey(td => td.Id);

        builder.Property(td => td.TaskId)
            .IsRequired();

        builder.Property(td => td.DependsOnTaskId)
            .IsRequired();

        builder.Property(td => td.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(td => td.CreatedAt)
            .IsRequired();

        builder.HasOne(td => td.Task)
            .WithMany(t => t.Dependencies)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(td => td.DependsOnTask)
            .WithMany()
            .HasForeignKey(td => td.DependsOnTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(td => new { td.TaskId, td.DependsOnTaskId }).IsUnique();
        builder.HasIndex(td => td.DependsOnTaskId);
    }
}
