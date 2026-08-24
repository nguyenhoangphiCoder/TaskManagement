using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public TaskMemberType MemberType { get; init; } = TaskMemberType.Assignee;
    public Guid TenantId { get; init; }
    public Guid AssignedBy { get; init; }
}
