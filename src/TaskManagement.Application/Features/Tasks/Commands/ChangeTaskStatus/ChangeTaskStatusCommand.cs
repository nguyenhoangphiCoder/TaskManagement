using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;

public record ChangeTaskStatusCommand : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public Guid NewStatusId { get; init; }
    public Guid TenantId { get; init; }
    public Guid ChangedBy { get; init; }
}
