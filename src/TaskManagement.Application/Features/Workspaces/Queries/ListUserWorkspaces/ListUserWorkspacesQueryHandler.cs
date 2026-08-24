using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagement.Application.Features.Workspaces.Queries.ListUserWorkspaces;

public class ListUserWorkspacesQueryHandler : IRequestHandler<ListUserWorkspacesQuery, Result<IEnumerable<WorkspaceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListUserWorkspacesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<WorkspaceDto>>> Handle(ListUserWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await _unitOfWork.Workspaces.GetUserWorkspacesAsync(request.UserId, cancellationToken);
        
        var workspacesInTenant = workspaces.Where(w => w.TenantId == request.TenantId).ToList();

        var dtos = workspacesInTenant.Select(w => 
        {
            var dto = w.Adapt<WorkspaceDto>();
            return dto with 
            {
                Slug = w.Slug.Value,
                MemberCount = w.Members.Count,
                ProjectCount = w.Projects.Count
            };
        });

        return Result.Success(dtos);
    }
}
