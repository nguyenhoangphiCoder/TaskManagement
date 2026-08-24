using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskNotificationService _notificationService;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork, ITaskNotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // Get task with details
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Access denied");

        // Validate updater exists and can access the workspace
        var updater = await _unitOfWork.Users.GetByIdAsync(request.UpdatedBy, cancellationToken);

        if (updater == null)
            return Result.Failure<TaskDto>("Updater not found");

        if (updater.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Updater does not belong to the same tenant");

        var updaterHasWorkspaceAccess = await _unitOfWork.WorkspaceMembers.AnyAsync(
            wm => wm.WorkspaceId == task.WorkspaceId && wm.UserId == request.UpdatedBy,
            cancellationToken);

        if (!updaterHasWorkspaceAccess)
            return Result.Failure<TaskDto>("Updater does not have access to this workspace");

        // Update title and description
        if (request.Title != null || request.Description != null)
        {
            task.UpdateDetails(request.Title ?? task.Title, request.Description ?? task.Description);
        }

        // Update priority
        if (request.Priority.HasValue)
        {
            task.ChangePriority(request.Priority.Value);
        }

        // Update dates
        if (request.StartDate.HasValue || request.DueDate.HasValue)
        {
            var startDate = request.StartDate ?? task.StartDate;
            var dueDate = request.DueDate ?? task.DueDate;
            task.SetDates(startDate, dueDate);
        }

        // Update estimate
        if (request.EstimatedMinutes.HasValue)
        {
            task.SetEstimate(request.EstimatedMinutes.Value);
        }

        // Save changes
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Broadcast Real-time Update
        await _notificationService.NotifyTaskUpdatedAsync(task.Id, task.WorkspaceId, cancellationToken);

        // Map to DTO
        var taskDto = task.Adapt<TaskDto>();
        taskDto = taskDto with
        {
            Code = task.Code.Value,
            StatusName = task.Status.Name,
            ProjectName = task.Project.Name,
            IsOverdue = task.IsOverdue()
        };

        return Result.Success(taskDto);
    }
}
