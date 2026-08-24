using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskReminderConfiguration : IEntityTypeConfiguration<TaskReminder>
{
    public void Configure(EntityTypeBuilder<TaskReminder> builder)
    {
        builder.ToTable("TaskReminders");

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.TaskId)
            .IsRequired();

        builder.Property(tr => tr.UserId)
            .IsRequired();

        builder.Property(tr => tr.RemindAt)
            .IsRequired();

        builder.Property(tr => tr.IsSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tr => tr.SentAt)
            .IsRequired(false);

        builder.HasOne(tr => tr.Task)
            .WithMany()
            .HasForeignKey(tr => tr.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tr => tr.User)
            .WithMany()
            .HasForeignKey(tr => tr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tr => new { tr.IsSent, tr.RemindAt });
        builder.HasIndex(tr => tr.TaskId);
    }
}
