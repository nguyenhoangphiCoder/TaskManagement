using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

public record TaskDetailDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    
    public Guid StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public string StatusColor { get; init; } = string.Empty;
    
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    
    public int EstimatedMinutes { get; init; }
    public int ActualMinutes { get; init; }
    
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string ProjectCode { get; init; } = string.Empty;
    
    public Guid? ParentTaskId { get; init; }
    public int SubtaskLevel { get; init; }
    
    public List<TaskMemberDto> Members { get; init; } = new();
    public List<TaskDependencyDto> Dependencies { get; init; } = new();
    public List<TaskChecklistDto> Checklists { get; init; } = new();
    public List<TaskCommentDto> Comments { get; init; } = new();
    public List<TaskAttachmentDto> Attachments { get; init; } = new();
    public List<TimeEntryDto> TimeEntries { get; init; } = new();
    public List<TaskDto> Subtasks { get; init; } = new();
    
    public DateTime CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
}

public record TaskDependencyDto
{
    public Guid DependsOnTaskId { get; init; }
    public string DependsOnTaskCode { get; init; } = string.Empty;
    public string DependsOnTaskTitle { get; init; } = string.Empty;
    public DependencyType Type { get; init; }
}

public record TaskAttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public Guid UploadedBy { get; init; }
    public string UploaderName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record TimeEntryDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int DurationMinutes { get; init; }
    public string? Notes { get; init; }
    public bool IsRunning { get; init; }
}
