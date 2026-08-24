using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Commands.RemoveMember;

public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure("Workspace not found");

        // A user can remove themselves, or an admin/owner can remove them
        var remover = workspace.Members.FirstOrDefault(m => m.UserId == request.RemovedBy);
        if (remover == null)
            return Result.Failure("You are not part of this workspace");

        if (request.RemovedBy != request.UserIdToRemove && remover.Role != MemberRole.Owner && remover.Role != MemberRole.Admin)
            return Result.Failure("You do not have permission to remove this member");

        try
        {
            workspace.RemoveMember(request.UserIdToRemove);
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
