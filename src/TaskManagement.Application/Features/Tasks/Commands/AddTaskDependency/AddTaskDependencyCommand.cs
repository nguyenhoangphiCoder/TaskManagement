using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.AddTaskDependency;

public record AddTaskDependencyCommand : IRequest<Result<bool>>
{
    public Guid TaskId { get; init; }
    public Guid DependsOnTaskId { get; init; }
    public DependencyType DependencyType { get; init; } = DependencyType.FinishToStart;
    public Guid TenantId { get; init; }
    public Guid CreatedBy { get; init; }
}
