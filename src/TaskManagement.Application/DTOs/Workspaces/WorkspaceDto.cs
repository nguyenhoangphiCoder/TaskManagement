using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Workspaces;

public record WorkspaceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public WorkspaceStatus Status { get; init; }
    public bool IsPrivate { get; init; }
    public int MemberCount { get; init; }
    public int ProjectCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
