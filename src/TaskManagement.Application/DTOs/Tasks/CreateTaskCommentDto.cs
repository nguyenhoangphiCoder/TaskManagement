using System;

namespace TaskManagement.Application.DTOs.Tasks;

public record CreateTaskCommentDto
{
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}
