using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");

        builder.HasKey(te => te.Id);

        builder.Property(te => te.TaskId)
            .IsRequired();

        builder.Property(te => te.UserId)
            .IsRequired();

        builder.Property(te => te.StartTime)
            .IsRequired();

        builder.Property(te => te.EndTime)
            .IsRequired(false);

        builder.Property(te => te.DurationMinutes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(te => te.Notes)
            .HasMaxLength(1000);

        builder.Property(te => te.IsRunning)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(te => te.Task)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.User)
            .WithMany()
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(te => te.TaskId);
        builder.HasIndex(te => te.UserId);
        builder.HasIndex(te => te.IsRunning);
    }
}
