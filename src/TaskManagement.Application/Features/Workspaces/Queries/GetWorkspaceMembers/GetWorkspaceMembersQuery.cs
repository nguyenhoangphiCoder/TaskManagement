using MediatR;
using System;
using System.Collections.Generic;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceMembers;

public record GetWorkspaceMembersQuery(Guid WorkspaceId, Guid TenantId, Guid UserId) : IRequest<Result<IEnumerable<WorkspaceMemberDto>>>;
