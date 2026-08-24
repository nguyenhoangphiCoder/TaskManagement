using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers");

        builder.HasKey(wm => wm.Id);

        builder.Property(wm => wm.WorkspaceId)
            .IsRequired();

        builder.Property(wm => wm.UserId)
            .IsRequired();

        builder.Property(wm => wm.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(wm => wm.JoinedAt)
            .IsRequired();

        builder.HasOne(wm => wm.Workspace)
            .WithMany(w => w.Members)
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wm => wm.User)
            .WithMany()
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wm => new { wm.WorkspaceId, wm.UserId }).IsUnique();
        builder.HasIndex(wm => wm.Role);
    }
}
