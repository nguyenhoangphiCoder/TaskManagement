using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.ToggleChecklist;

public class ToggleChecklistCommandHandler : IRequestHandler<ToggleChecklistCommand, Result<TaskChecklistDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleChecklistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskChecklistDto>> Handle(ToggleChecklistCommand request, CancellationToken cancellationToken)
    {
        // Get task to validate tenant
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskChecklistDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskChecklistDto>("Access denied");

        // Get checklist item
        var checklistItem = await _unitOfWork.TaskChecklists.GetByIdAsync(request.ChecklistItemId, cancellationToken);
        
        if (checklistItem == null)
            return Result.Failure<TaskChecklistDto>("Checklist item not found");

        // Validate it belongs to the task
        if (checklistItem.TaskId != request.TaskId)
            return Result.Failure<TaskChecklistDto>("Checklist item does not belong to this task");

        // Toggle completion
        checklistItem.Toggle();

        _unitOfWork.TaskChecklists.Update(checklistItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = checklistItem.Adapt<TaskChecklistDto>();

        return Result.Success(dto);
    }
}
