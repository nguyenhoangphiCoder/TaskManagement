using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public TaskManagement.Domain.Enums.TaskPriority? Priority { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public int? EstimatedMinutes { get; init; }
    public Guid TenantId { get; init; }
    public Guid UpdatedBy { get; init; }
}
