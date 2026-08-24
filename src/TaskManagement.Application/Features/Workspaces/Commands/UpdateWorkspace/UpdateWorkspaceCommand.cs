using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;

namespace TaskManagement.Application.Features.Workspaces.Commands.UpdateWorkspace;

public record UpdateWorkspaceCommand : IRequest<Result<WorkspaceDto>>
{
    public Guid WorkspaceId { get; init; }
    public UpdateWorkspaceDto Workspace { get; init; } = null!;
    public Guid TenantId { get; init; }
    public Guid UpdatedBy { get; init; }
}
