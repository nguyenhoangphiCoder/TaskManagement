using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Queries.ListTasks;

public class ListTasksQueryHandler : IRequestHandler<ListTasksQuery, Result<PaginatedList<TaskDto>>>
{
    private readonly ITaskRepository _taskRepository;

    public ListTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<PaginatedList<TaskDto>>> Handle(ListTasksQuery request, CancellationToken cancellationToken)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filteredTasks = allTasks.AsEnumerable();

        if (request.ProjectId.HasValue)
            filteredTasks = filteredTasks.Where(t => t.ProjectId == request.ProjectId.Value);

        if (request.WorkspaceId.HasValue)
            filteredTasks = filteredTasks.Where(t => t.WorkspaceId == request.WorkspaceId.Value);

        if (request.AssignedToUserId.HasValue)
            filteredTasks = filteredTasks.Where(t => t.Members.Any(m => m.UserId == request.AssignedToUserId.Value));

        if (request.StatusId.HasValue)
            filteredTasks = filteredTasks.Where(t => t.StatusId == request.StatusId.Value);

        if (request.Priority.HasValue)
            filteredTasks = filteredTasks.Where(t => t.Priority == request.Priority.Value);

        if (request.IsOverdue.HasValue && request.IsOverdue.Value)
            filteredTasks = filteredTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            filteredTasks = filteredTasks.Where(t => 
                t.Title.ToLower().Contains(searchLower) || 
                (t.Description != null && t.Description.ToLower().Contains(searchLower)));
        }

        filteredTasks = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending ? filteredTasks.OrderByDescending(t => t.Title) : filteredTasks.OrderBy(t => t.Title),
            "priority" => request.SortDescending ? filteredTasks.OrderByDescending(t => t.Priority) : filteredTasks.OrderBy(t => t.Priority),
            "duedate" => request.SortDescending ? filteredTasks.OrderByDescending(t => t.DueDate) : filteredTasks.OrderBy(t => t.DueDate),
            "sortorder" => request.SortDescending ? filteredTasks.OrderByDescending(t => t.SortOrder) : filteredTasks.OrderBy(t => t.SortOrder),
            _ => request.SortDescending ? filteredTasks.OrderByDescending(t => t.CreatedAt) : filteredTasks.OrderBy(t => t.CreatedAt)
        };

        var totalCount = filteredTasks.Count();

        var pagedTasks = filteredTasks
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var detailedTasks = new List<Domain.Entities.Task>();
        foreach (var task in pagedTasks)
        {
            var detailedTask = await _taskRepository.GetByIdWithDetailsAsync(task.Id, cancellationToken);
            detailedTasks.Add(detailedTask ?? task);
        }

        // Map to DTOs with status/project names from the detailed task graph
        var taskDtos = detailedTasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Code = t.Code.Value,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            StatusId = t.StatusId,
            StatusName = t.Status?.Name ?? string.Empty,
            StartDate = t.StartDate,
            DueDate = t.DueDate,
            IsOverdue = t.IsOverdue(),
            EstimatedMinutes = t.EstimatedMinutes,
            ActualMinutes = t.ActualMinutes,
            ProjectId = t.ProjectId,
            ProjectName = t.Project?.Name ?? string.Empty,
            ParentTaskId = t.ParentTaskId,
            SubtaskLevel = t.SubtaskLevel,
            Members = t.Members.Select(m => new TaskMemberDto
            {
                UserId = m.UserId,
                FullName = m.User?.FullName ?? string.Empty,
                Email = m.User?.Email ?? string.Empty,
                MemberType = m.MemberType
            }).ToList(),
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        var paginatedList = PaginatedList<TaskDto>.Create(
            taskDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(paginatedList);
    }
}
