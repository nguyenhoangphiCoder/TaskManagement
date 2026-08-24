using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceMembers;

public class GetWorkspaceMembersQueryHandler : IRequestHandler<GetWorkspaceMembersQuery, Result<IEnumerable<WorkspaceMemberDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWorkspaceMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<WorkspaceMemberDto>>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _unitOfWork.Workspaces.GetByIdWithMembersAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure<IEnumerable<WorkspaceMemberDto>>("Workspace not found");

        if (workspace.IsPrivate && !workspace.Members.Any(m => m.UserId == request.UserId))
            return Result.Failure<IEnumerable<WorkspaceMemberDto>>("Access denied");

        var members = workspace.Members.Select(m => new WorkspaceMemberDto
        {
            UserId = m.UserId,
            FullName = m.User?.FullName ?? "Unknown",
            Email = m.User?.Email ?? "Unknown",
            AvatarUrl = m.User?.AvatarUrl,
            Role = m.Role,
            JoinedAt = m.JoinedAt
        }).OrderBy(m => m.Role).ThenBy(m => m.FullName).ToList();

        return Result.Success<IEnumerable<WorkspaceMemberDto>>(members);
    }
}
