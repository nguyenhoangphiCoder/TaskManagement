using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public CreateTaskDto Task { get; init; } = null!;
    public Guid TenantId { get; init; }
    public Guid CreatedBy { get; init; }
}
