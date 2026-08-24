using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Workspaces.Commands.ArchiveWorkspace;

public record ArchiveWorkspaceCommand : IRequest<Result>
{
    public Guid WorkspaceId { get; init; }
    public Guid TenantId { get; init; }
    public Guid ArchivedBy { get; init; }
}
