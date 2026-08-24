using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskActivityConfiguration : IEntityTypeConfiguration<TaskActivity>
{
    public void Configure(EntityTypeBuilder<TaskActivity> builder)
    {
        builder.ToTable("TaskActivities");

        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.Property(ta => ta.UserId)
            .IsRequired();

        builder.Property(ta => ta.ActivityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ta => ta.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ta => ta.OldValue)
            .HasMaxLength(500);

        builder.Property(ta => ta.NewValue)
            .HasMaxLength(500);

        builder.Property(ta => ta.CreatedAt)
            .IsRequired();

        builder.HasOne(ta => ta.Task)
            .WithMany()
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.User)
            .WithMany()
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ta => ta.TaskId);
        builder.HasIndex(ta => ta.CreatedAt);
    }
}
