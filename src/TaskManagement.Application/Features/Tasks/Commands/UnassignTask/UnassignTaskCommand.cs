using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.UnassignTask;

public record UnassignTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public Guid UnassignedBy { get; init; }
}
