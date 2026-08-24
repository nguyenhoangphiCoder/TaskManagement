using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskMemberConfiguration : IEntityTypeConfiguration<TaskMember>
{
    public void Configure(EntityTypeBuilder<TaskMember> builder)
    {
        builder.ToTable("TaskMembers");

        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.TaskId)
            .IsRequired();

        builder.Property(tm => tm.UserId)
            .IsRequired();

        builder.Property(tm => tm.MemberType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(tm => tm.AssignedAt)
            .IsRequired();

        builder.HasOne(tm => tm.Task)
            .WithMany(t => t.Members)
            .HasForeignKey(tm => tm.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.User)
            .WithMany()
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tm => new { tm.TaskId, tm.UserId, tm.MemberType }).IsUnique();
    }
}
