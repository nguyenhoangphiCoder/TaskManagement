using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

public record TaskDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public Guid StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public int EstimatedMinutes { get; init; }
    public int ActualMinutes { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public Guid? ParentTaskId { get; init; }
    public int SubtaskLevel { get; init; }
    public List<TaskMemberDto> Members { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record TaskMemberDto
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public TaskMemberType MemberType { get; init; }
}
