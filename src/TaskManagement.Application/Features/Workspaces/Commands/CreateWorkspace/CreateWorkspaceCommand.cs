using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;

namespace TaskManagement.Application.Features.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand : IRequest<Result<WorkspaceDto>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPrivate { get; init; }
    public Guid TenantId { get; init; }
    public Guid OwnerId { get; init; }
}
