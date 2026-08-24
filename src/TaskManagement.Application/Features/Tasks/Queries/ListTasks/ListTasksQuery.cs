using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Queries.ListTasks;

public record ListTasksQuery : IRequest<Result<PaginatedList<TaskDto>>>
{
    public Guid? ProjectId { get; init; }
    public Guid? WorkspaceId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public Guid? StatusId { get; init; }
    public TaskPriority? Priority { get; init; }
    public bool? IsOverdue { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}
