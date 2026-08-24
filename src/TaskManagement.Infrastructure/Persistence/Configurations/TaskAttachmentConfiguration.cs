using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.ToTable("TaskAttachments");

        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.Property(ta => ta.UploadedBy)
            .IsRequired();

        builder.Property(ta => ta.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ta => ta.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ta => ta.FileSizeBytes)
            .IsRequired();

        builder.HasOne(ta => ta.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.Uploader)
            .WithMany()
            .HasForeignKey(ta => ta.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ta => ta.TaskId);
        builder.HasIndex(ta => ta.CreatedAt);
    }
}
