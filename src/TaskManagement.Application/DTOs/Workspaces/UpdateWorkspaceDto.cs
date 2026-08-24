namespace TaskManagement.Application.DTOs.Workspaces;

public record UpdateWorkspaceDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
