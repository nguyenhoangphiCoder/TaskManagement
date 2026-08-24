using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class RecurringTaskTemplateConfiguration : IEntityTypeConfiguration<RecurringTaskTemplate>
{
    public void Configure(EntityTypeBuilder<RecurringTaskTemplate> builder)
    {
        builder.ToTable("RecurringTaskTemplates");

        builder.HasKey(rtt => rtt.Id);

        builder.Property(rtt => rtt.TenantId)
            .IsRequired();

        builder.Property(rtt => rtt.WorkspaceId)
            .IsRequired();

        builder.Property(rtt => rtt.ProjectId)
            .IsRequired();

        builder.Property(rtt => rtt.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rtt => rtt.Description)
            .HasMaxLength(5000);

        builder.Property(rtt => rtt.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(rtt => rtt.StatusId)
            .IsRequired();

        builder.Property(rtt => rtt.RecurrenceRule)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rtt => rtt.StartDate)
            .IsRequired();

        builder.Property(rtt => rtt.EndDate)
            .IsRequired(false);

        builder.Property(rtt => rtt.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(rtt => rtt.LastGeneratedAt)
            .IsRequired(false);

        builder.HasOne(rtt => rtt.Project)
            .WithMany()
            .HasForeignKey(rtt => rtt.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rtt => rtt.Workspace)
            .WithMany()
            .HasForeignKey(rtt => rtt.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rtt => new { rtt.IsActive, rtt.StartDate });
        builder.HasIndex(rtt => rtt.ProjectId);
    }
}
