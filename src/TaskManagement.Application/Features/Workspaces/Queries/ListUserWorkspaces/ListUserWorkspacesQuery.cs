using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using System.Collections.Generic;

namespace TaskManagement.Application.Features.Workspaces.Queries.ListUserWorkspaces;

public record ListUserWorkspacesQuery(Guid TenantId, Guid UserId) : IRequest<Result<IEnumerable<WorkspaceDto>>>;
