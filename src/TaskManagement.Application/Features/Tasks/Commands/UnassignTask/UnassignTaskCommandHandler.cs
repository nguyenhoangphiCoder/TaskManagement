using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UnassignTask;

public class UnassignTaskCommandHandler : IRequestHandler<UnassignTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnassignTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskDto>> Handle(UnassignTaskCommand request, CancellationToken cancellationToken)
    {
        // Get task with members
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskDto>("Access denied");

        // Remove member from task
        task.RemoveMember(request.UserId);

        // Save changes
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
                MemberType = m.MemberType
            }).ToList()
        };

        return Result.Success(taskDto);
    }
}
