using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // Get task
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<bool>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<bool>("Access denied");

        // Check if task has subtasks (optional business rule - can be removed if you want to allow cascading delete)
        var hasSubtasks = await _unitOfWork.Tasks.AnyAsync(
            t => t.ParentTaskId == request.TaskId,
            cancellationToken);

        if (hasSubtasks)
            return Result.Failure<bool>("Cannot delete task with subtasks. Delete subtasks first.");

        // Soft delete (BaseAuditableEntity handles this)
        _unitOfWork.Tasks.Remove(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
