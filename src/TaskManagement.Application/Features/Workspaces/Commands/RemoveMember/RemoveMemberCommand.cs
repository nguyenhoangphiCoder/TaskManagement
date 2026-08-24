using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Workspaces.Commands.RemoveMember;

public record RemoveMemberCommand : IRequest<Result>
{
    public Guid WorkspaceId { get; init; }
    public Guid UserIdToRemove { get; init; }
    public Guid TenantId { get; init; }
    public Guid RemovedBy { get; init; }
}
