using System;

namespace TaskManagement.Application.DTOs.Tasks;

public record TaskChecklistDto
{
    public Guid Id { get; init; }
    public Guid TaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public int Position { get; init; }
    public DateTime CreatedAt { get; init; }
}
