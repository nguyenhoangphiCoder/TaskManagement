using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        // Get task to validate tenant
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<bool>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<bool>("Access denied");

        // Get comment
        var comment = await _unitOfWork.TaskComments.GetByIdAsync(request.CommentId, cancellationToken);
        
        if (comment == null)
            return Result.Failure<bool>("Comment not found");

        // Validate comment belongs to task
        if (comment.TaskId != request.TaskId)
            return Result.Failure<bool>("Comment does not belong to this task");

        // Optional: Validate user can delete (either author or admin)
        // This can be enhanced with proper authorization
        // For now, we soft delete the comment
        _unitOfWork.TaskComments.Remove(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
