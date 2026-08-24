using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.RemoveTaskDependency;

public record RemoveTaskDependencyCommand : IRequest<Result<bool>>
{
    public Guid TaskId { get; init; }
    public Guid DependsOnTaskId { get; init; }
    public Guid TenantId { get; init; }
    public Guid DeletedBy { get; init; }
}
