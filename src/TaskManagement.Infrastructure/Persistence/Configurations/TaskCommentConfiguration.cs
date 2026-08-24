using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("TaskComments");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.TaskId)
            .IsRequired();

        builder.Property(tc => tc.AuthorId)
            .IsRequired();

        builder.Property(tc => tc.ParentCommentId)
            .IsRequired(false);

        builder.Property(tc => tc.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.HasOne(tc => tc.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tc => tc.Author)
            .WithMany()
            .HasForeignKey(tc => tc.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tc => tc.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(tc => tc.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tc => tc.TaskId);
        builder.HasIndex(tc => tc.ParentCommentId);
        builder.HasIndex(tc => tc.CreatedAt);
    }
}
