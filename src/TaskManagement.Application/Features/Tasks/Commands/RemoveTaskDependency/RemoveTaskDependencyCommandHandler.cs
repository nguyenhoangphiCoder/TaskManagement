using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.RemoveTaskDependency;

public class RemoveTaskDependencyCommandHandler : IRequestHandler<RemoveTaskDependencyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTaskDependencyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveTaskDependencyCommand request, CancellationToken cancellationToken)
    {
        // Get the task to validate tenant access
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<bool>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<bool>("Access denied");

        // Find the dependency
        var dependency = await _unitOfWork.TaskDependencies.FirstOrDefaultAsync(
            td => td.TaskId == request.TaskId && td.DependsOnTaskId == request.DependsOnTaskId,
            cancellationToken);

        if (dependency == null)
            return Result.Failure<bool>("Dependency not found");

        // Remove dependency
        _unitOfWork.TaskDependencies.Remove(dependency);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
