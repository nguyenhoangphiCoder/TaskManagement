using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.WorkspaceId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.OwnsOne(p => p.Timeline, timeline =>
        {
            timeline.Property(t => t.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            timeline.Property(t => t.EndDate)
                .HasColumnName("EndDate")
                .IsRequired(false);
        });

        builder.Property(p => p.ProgressPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(p => p.IsPublicToWorkspace)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(p => p.Workspace)
            .WithMany(w => w.Projects)
            .HasForeignKey(p => p.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Members)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.WorkspaceId, p.Code }).IsUnique();
        builder.HasIndex(p => p.Status);
    }
}
