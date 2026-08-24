using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands.AddChecklist;

public class AddChecklistCommandHandler : IRequestHandler<AddChecklistCommand, Result<TaskChecklistDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddChecklistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskChecklistDto>> Handle(AddChecklistCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null) return Result.Failure<TaskChecklistDto>("Task not found");
        if (task.TenantId != request.TenantId) return Result.Failure<TaskChecklistDto>("Access denied");

        var checklistItem = TaskChecklist.Create(
            request.TaskId,
            request.Checklist.Title,
            request.Checklist.Position);

        await _unitOfWork.TaskChecklists.AddAsync(checklistItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = checklistItem.Adapt<TaskChecklistDto>();
        return Result.Success(dto);
    }
}
