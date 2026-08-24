using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceStatuses;

public class GetWorkspaceStatusesQueryHandler : IRequestHandler<GetWorkspaceStatusesQuery, Result<IEnumerable<TaskStatusDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWorkspaceStatusesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<TaskStatusDto>>> Handle(GetWorkspaceStatusesQuery request, CancellationToken cancellationToken)
    {
        // Check if user is a member of the workspace
        var isMember = await _unitOfWork.WorkspaceMembers.AnyAsync(
            wm => wm.WorkspaceId == request.WorkspaceId && wm.UserId == request.UserId,
            cancellationToken);

        if (!isMember)
            return Result.Failure<IEnumerable<TaskStatusDto>>("Workspace not found or access denied.");

        var statuses = await _unitOfWork.TaskStatuses.FindAsync(
            s => s.WorkspaceId == request.WorkspaceId && s.TenantId == request.TenantId,
            cancellationToken);

        var dtos = statuses.Select(s => new TaskStatusDto
        {
            Id = s.Id,
            Name = s.Name,
            Color = s.Color,
            Position = s.Position,
            IsDefault = s.IsDefault
        }).OrderBy(s => s.Position).ToList();

        return Result.Success<IEnumerable<TaskStatusDto>>(dtos);
    }
}
