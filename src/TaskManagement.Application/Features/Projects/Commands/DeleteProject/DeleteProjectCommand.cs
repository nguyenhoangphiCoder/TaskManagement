using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand : IRequest<Result<bool>>
{
    public Guid ProjectId { get; init; }
    public Guid TenantId { get; init; }
    public Guid DeletedBy { get; init; }
}
