using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskAttachment : BaseAuditableEntity
{
    public Guid TaskId { get; private set; }
    public Guid UploadedBy { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }

    public Task Task { get; private set; } = null!;
    public User Uploader { get; private set; } = null!;

    private TaskAttachment() { }

    public static TaskAttachment Create(
        Guid taskId,
        Guid uploadedBy,
        string fileName,
        string fileUrl,
        string contentType,
        long fileSizeBytes)
    {
        return new TaskAttachment
        {
            TaskId = taskId,
            UploadedBy = uploadedBy,
            FileName = fileName,
            FileUrl = fileUrl,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes
        };
    }
}
