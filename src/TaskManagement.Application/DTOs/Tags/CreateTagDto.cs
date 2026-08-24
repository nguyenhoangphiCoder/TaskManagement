using System;

namespace TaskManagement.Application.DTOs.Tags;

public record CreateTagDto
{
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#gray";
}
