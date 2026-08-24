using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands.AddChecklistItem;

public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, Result<TaskChecklistDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddChecklistItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskChecklistDto>> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // Get task
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskChecklistDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskChecklistDto>("Access denied");

        // Create checklist item
        var checklistItem = TaskChecklist.Create(
            request.TaskId,
            request.Title,
            request.Position);

        await _unitOfWork.TaskChecklists.AddAsync(checklistItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = checklistItem.Adapt<TaskChecklistDto>();

        return Result.Success(dto);
    }
}
