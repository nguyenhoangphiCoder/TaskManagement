using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Services;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskCodeGenerationService _codeService;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ITaskCodeGenerationService codeService)
    {
        _unitOfWork = unitOfWork;
        _codeService = codeService;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Validate project exists
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Task.ProjectId, cancellationToken);
        if (project == null || project.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Project not found");

        // Validate status exists
        var status = await _unitOfWork.TaskStatuses.FirstOrDefaultAsync(
            s => s.Id == request.Task.StatusId,
            cancellationToken);
        
        if (status == null || status.TenantId != request.TenantId || status.WorkspaceId != project.WorkspaceId)
            return Result.Failure<TaskDto>("Task status not found");

        Domain.Entities.Task? parentTask = null;
        if (request.Task.ParentTaskId.HasValue)
        {
            parentTask = await _unitOfWork.Tasks.GetByIdAsync(request.Task.ParentTaskId.Value, cancellationToken);

            if (parentTask == null)
                return Result.Failure<TaskDto>("Parent task not found");

            if (parentTask.TenantId != request.TenantId || parentTask.WorkspaceId != project.WorkspaceId)
                return Result.Failure<TaskDto>("Parent task must belong to the same workspace");

            if (parentTask.ProjectId != request.Task.ProjectId)
                return Result.Failure<TaskDto>("Parent task must belong to the same project");
        }

        // Generate task code
        var sequence = await _unitOfWork.Tasks.GetNextTaskSequenceAsync(request.Task.ProjectId, cancellationToken);
        var taskCode = _codeService.GenerateTaskCode(project.Code, sequence);

        // Create task entity
        var task = Domain.Entities.Task.Create(
            request.TenantId,
            project.WorkspaceId,
            request.Task.ProjectId,
            request.Task.Title,
            taskCode,
            request.Task.StatusId,
            request.Task.Priority);

        if (parentTask != null)
        {
            task.SetParentTask(parentTask.Id, parentTask.SubtaskLevel);
        }

        // Set optional properties
        if (!string.IsNullOrEmpty(request.Task.Description))
            task.UpdateDetails(request.Task.Title, request.Task.Description);

        if (request.Task.StartDate.HasValue || request.Task.DueDate.HasValue)
            task.SetDates(request.Task.StartDate, request.Task.DueDate);

        if (request.Task.EstimatedMinutes > 0)
            task.SetEstimate(request.Task.EstimatedMinutes);

        // Assign members
        foreach (var userId in request.Task.AssigneeIds)
        {
            task.AssignMember(userId, TaskMemberType.Assignee);
        }

        // Add task
        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var taskDto = task.Adapt<TaskDto>();
        taskDto = taskDto with
        {
            Code = task.Code.Value,
            StatusName = status.Name,
            ProjectName = project.Name,
            ParentTaskId = task.ParentTaskId,
            SubtaskLevel = task.SubtaskLevel,
            IsOverdue = task.IsOverdue()
        };

        return Result.Success(taskDto);
    }
}
