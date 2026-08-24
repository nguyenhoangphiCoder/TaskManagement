using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Workspaces.Commands.InviteMember;

public record InviteMemberCommand : IRequest<Result>
{
    public Guid WorkspaceId { get; init; }
    public Guid UserIdToInvite { get; init; }
    public MemberRole Role { get; init; }
    public Guid TenantId { get; init; }
    public Guid InvitedBy { get; init; }
}
