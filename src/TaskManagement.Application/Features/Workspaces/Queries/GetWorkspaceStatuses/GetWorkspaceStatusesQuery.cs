using MediatR;
using System;
using System.Collections.Generic;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceStatuses;

public record GetWorkspaceStatusesQuery(Guid WorkspaceId, Guid TenantId, Guid UserId) : IRequest<Result<IEnumerable<TaskStatusDto>>>;
