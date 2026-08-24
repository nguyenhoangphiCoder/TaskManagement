using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Commands.InviteMember;

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public InviteMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure("Workspace not found");

        var inviter = workspace.Members.FirstOrDefault(m => m.UserId == request.InvitedBy);
        if (inviter == null || (inviter.Role != MemberRole.Owner && inviter.Role != MemberRole.Admin))
            return Result.Failure("Only admins and owners can invite members");

        // Validate user to invite exists
        var userToInvite = await _unitOfWork.Users.GetByIdAsync(request.UserIdToInvite, cancellationToken);
        if (userToInvite == null || userToInvite.TenantId != request.TenantId)
            return Result.Failure("User to invite not found");

        try
        {
            workspace.InviteMember(request.UserIdToInvite, request.Role);
            _unitOfWork.Workspaces.Update(workspace);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
