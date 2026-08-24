using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

public record CreateTaskDto
{
    public Guid ProjectId { get; init; }
    public Guid? ParentTaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public Guid StatusId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public int EstimatedMinutes { get; init; }
    public List<Guid> AssigneeIds { get; init; } = new();
    public List<string> Tags { get; init; } = new();
}
