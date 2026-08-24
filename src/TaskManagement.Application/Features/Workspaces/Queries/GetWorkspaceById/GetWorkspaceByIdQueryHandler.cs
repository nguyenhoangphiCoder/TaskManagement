using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.Interfaces;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceById;

public class GetWorkspaceByIdQueryHandler : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWorkspaceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceDto>> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.Id, cancellationToken);

        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure<WorkspaceDto>("Workspace not found");

        if (workspace.IsPrivate && !workspace.Members.Any(m => m.UserId == request.UserId))
            return Result.Failure<WorkspaceDto>("You do not have permission to view this workspace");

        var owner = await _unitOfWork.Users.GetByIdAsync(workspace.OwnerId, cancellationToken);

        var workspaceDto = workspace.Adapt<WorkspaceDto>();
        workspaceDto = workspaceDto with
        {
            Slug = workspace.Slug.Value,
            OwnerName = owner?.FullName ?? "Unknown",
            MemberCount = workspace.Members.Count,
            ProjectCount = workspace.Projects.Count
        };

        return Result.Success(workspaceDto);
    }
}
