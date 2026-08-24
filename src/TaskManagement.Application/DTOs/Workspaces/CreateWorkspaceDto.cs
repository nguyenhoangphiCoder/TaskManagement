namespace TaskManagement.Application.DTOs.Workspaces;

public record CreateWorkspaceDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPrivate { get; init; }
}
