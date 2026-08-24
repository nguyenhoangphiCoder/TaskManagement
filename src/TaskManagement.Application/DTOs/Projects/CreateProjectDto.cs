using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.DTOs.Projects;

public record CreateProjectDto
{
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsPublicToWorkspace { get; init; } = true;
}
