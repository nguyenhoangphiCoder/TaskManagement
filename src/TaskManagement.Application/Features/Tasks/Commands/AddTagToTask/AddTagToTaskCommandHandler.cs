using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using System.Linq;

namespace TaskManagement.Application.Features.Tasks.Commands.AddTagToTask;

public class AddTagToTaskCommandHandler : IRequestHandler<AddTagToTaskCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddTagToTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddTagToTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null) return Result.Failure("Task not found");
        if (task.TenantId != request.TenantId) return Result.Failure("Access denied");

        var tag = await _unitOfWork.Tags.GetByIdAsync(request.TagId, cancellationToken);
        if (tag == null || tag.WorkspaceId != task.WorkspaceId) return Result.Failure("Tag not found or invalid workspace");

        var taskTag = TaskTag.Create(request.TaskId, request.TagId);
        await _unitOfWork.TaskTags.AddAsync(taskTag, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
