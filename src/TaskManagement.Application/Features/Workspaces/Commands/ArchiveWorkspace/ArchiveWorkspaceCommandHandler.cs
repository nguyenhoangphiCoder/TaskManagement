using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Commands.ArchiveWorkspace;

public class ArchiveWorkspaceCommandHandler : IRequestHandler<ArchiveWorkspaceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveWorkspaceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure("Workspace not found");

        var member = workspace.Members.FirstOrDefault(m => m.UserId == request.ArchivedBy);
        if (member == null || member.Role != MemberRole.Owner)
            return Result.Failure("Only the workspace owner can archive it");

        workspace.Archive();

        _unitOfWork.Workspaces.Update(workspace);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
