using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskNotificationService _notificationService;

    public ChangeTaskStatusCommandHandler(IUnitOfWork unitOfWork, ITaskNotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<TaskDto>> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        // Get task
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Access denied");

        // Validate new status exists and belongs to same workspace
        var newStatus = await _unitOfWork.TaskStatuses.GetByIdAsync(request.NewStatusId, cancellationToken);
        
        if (newStatus == null)
            return Result.Failure<TaskDto>("Status not found");

        if (newStatus.WorkspaceId != task.WorkspaceId)
            return Result.Failure<TaskDto>("Status does not belong to the same workspace");

        // Change status
        task.ChangeStatus(request.NewStatusId);

        // Save changes
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Broadcast Real-time Update
        await _notificationService.NotifyTaskStatusChangedAsync(task.Id, task.WorkspaceId, request.NewStatusId, cancellationToken);

        // Map to DTO
        var taskDto = task.Adapt<TaskDto>();
        taskDto = taskDto with
        {
            Code = task.Code.Value,
            StatusName = newStatus.Name,
            ProjectName = task.Project.Name,
            IsOverdue = task.IsOverdue()
        };

        return Result.Success(taskDto);
    }
}
