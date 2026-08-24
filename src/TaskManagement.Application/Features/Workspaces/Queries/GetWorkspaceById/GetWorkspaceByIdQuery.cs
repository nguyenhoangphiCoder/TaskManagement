using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;

namespace TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceById;

public record GetWorkspaceByIdQuery(Guid Id, Guid TenantId, Guid UserId) : IRequest<Result<WorkspaceDto>>;
