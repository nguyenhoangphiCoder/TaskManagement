using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandHandler : IRequestHandler<UpdateWorkspaceCommand, Result<WorkspaceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkspaceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceDto>> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure<WorkspaceDto>("Workspace not found");

        // Verify that the user trying to update the workspace is an Owner or Admin
        var member = workspace.Members.FirstOrDefault(m => m.UserId == request.UpdatedBy);
        if (member == null || (member.Role != MemberRole.Owner && member.Role != MemberRole.Admin))
            return Result.Failure<WorkspaceDto>("You do not have permission to update this workspace");

        workspace.UpdateDetails(request.Workspace.Name, request.Workspace.Description);

        _unitOfWork.Workspaces.Update(workspace);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var workspaceDto = workspace.Adapt<WorkspaceDto>();
        workspaceDto = workspaceDto with
        {
            Slug = workspace.Slug.Value,
            MemberCount = workspace.Members.Count,
            ProjectCount = workspace.Projects.Count
        };

        return Result.Success(workspaceDto);
    }
}
