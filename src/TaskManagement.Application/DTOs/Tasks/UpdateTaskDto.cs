using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

public record UpdateTaskDto
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public TaskPriority? Priority { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public int? EstimatedMinutes { get; init; }
}
