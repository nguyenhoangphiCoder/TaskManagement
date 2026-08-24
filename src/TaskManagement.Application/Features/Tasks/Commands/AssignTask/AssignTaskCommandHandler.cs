using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskNotificationService _notificationService;

    public AssignTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ITaskNotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<TaskDto>> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        // Get task with members
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Access denied");

        // Validate assigner exists and can access the workspace
        var assigner = await _unitOfWork.Users.GetByIdAsync(request.AssignedBy, cancellationToken);

        if (assigner == null)
            return Result.Failure<TaskDto>("Assigner not found");

        if (assigner.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Assigner does not belong to the same tenant");

        var assignerHasWorkspaceAccess = await _unitOfWork.WorkspaceMembers.AnyAsync(
            wm => wm.WorkspaceId == task.WorkspaceId && wm.UserId == request.AssignedBy,
            cancellationToken);

        if (!assignerHasWorkspaceAccess)
            return Result.Failure<TaskDto>("Assigner does not have access to this workspace");

        // Validate user exists and belongs to the same tenant
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return Result.Failure<TaskDto>("User not found");

        if (user.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("User does not belong to the same tenant");

        // Check if user has access to this workspace (optional - can be enhanced)
        var hasWorkspaceAccess = await _unitOfWork.WorkspaceMembers.AnyAsync(
            wm => wm.WorkspaceId == task.WorkspaceId && wm.UserId == request.UserId,
            cancellationToken);

        if (!hasWorkspaceAccess)
            return Result.Failure<TaskDto>("User does not have access to this workspace");

        // Assign member to task
        task.AssignMember(request.UserId, request.MemberType);

        // Save changes
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyTaskUpdatedAsync(task.Id, task.WorkspaceId, cancellationToken);

        // Map to DTO
        var taskDto = task.Adapt<TaskDto>();
        taskDto = taskDto with
        {
            Code = task.Code.Value,
            StatusName = task.Status.Name,
            ProjectName = task.Project.Name,
            IsOverdue = task.IsOverdue(),
            Members = task.Members.Select(m => new TaskMemberDto
            {
                UserId = m.UserId,
                FullName = m.User?.FullName ?? string.Empty,
                Email = m.User?.Email ?? string.Empty,
                MemberType = m.MemberType
            }).ToList()
        };

        return Result.Success(taskDto);
    }
}
