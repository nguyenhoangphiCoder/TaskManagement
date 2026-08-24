using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
{
    public void Configure(EntityTypeBuilder<TaskTag> builder)
    {
        builder.ToTable("TaskTags");

        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.TaskId)
            .IsRequired();

        builder.Property(tt => tt.TagId)
            .IsRequired();

        builder.HasOne(tt => tt.Task)
            .WithMany()
            .HasForeignKey(tt => tt.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tt => tt.Tag)
            .WithMany()
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tt => new { tt.TaskId, tt.TagId }).IsUnique();
    }
}
