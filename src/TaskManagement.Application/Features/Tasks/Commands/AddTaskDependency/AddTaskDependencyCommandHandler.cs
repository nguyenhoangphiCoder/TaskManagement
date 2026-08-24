using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Services;

namespace TaskManagement.Application.Features.Tasks.Commands.AddTaskDependency;

public class AddTaskDependencyCommandHandler : IRequestHandler<AddTaskDependencyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskDependencyValidationService _dependencyValidationService;

    public AddTaskDependencyCommandHandler(
        IUnitOfWork unitOfWork,
        ITaskDependencyValidationService dependencyValidationService)
    {
        _unitOfWork = unitOfWork;
        _dependencyValidationService = dependencyValidationService;
    }

    public async Task<Result<bool>> Handle(AddTaskDependencyCommand request, CancellationToken cancellationToken)
    {
        // Get both tasks
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        var dependsOnTask = await _unitOfWork.Tasks.GetByIdAsync(request.DependsOnTaskId, cancellationToken);

        if (task == null || dependsOnTask == null)
            return Result.Failure<bool>("One or both tasks not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId || dependsOnTask.TenantId != request.TenantId)
            return Result.Failure<bool>("Access denied");

        // Validate both tasks are in the same workspace
        if (task.WorkspaceId != dependsOnTask.WorkspaceId)
            return Result.Failure<bool>("Tasks must be in the same workspace");

        // Check if dependency already exists
        var existingDependency = await _unitOfWork.TaskDependencies.FirstOrDefaultAsync(
            td => td.TaskId == request.TaskId && td.DependsOnTaskId == request.DependsOnTaskId,
            cancellationToken);

        if (existingDependency != null)
            return Result.Failure<bool>("Dependency already exists");

        // Validate no circular dependency using DFS algorithm
        var isValid = await _dependencyValidationService.ValidateNoCycleAsync(
            request.TaskId,
            request.DependsOnTaskId,
            cancellationToken);

        if (!isValid)
            return Result.Failure<bool>("Adding this dependency would create a circular dependency");

        // Create dependency
        var dependency = TaskDependency.Create(
            request.TaskId,
            request.DependsOnTaskId,
            request.DependencyType);

        await _unitOfWork.TaskDependencies.AddAsync(dependency, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
