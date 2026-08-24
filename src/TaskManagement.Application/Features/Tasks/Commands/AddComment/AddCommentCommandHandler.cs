using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<TaskCommentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskNotificationService _notificationService;

    public AddCommentCommandHandler(IUnitOfWork unitOfWork, ITaskNotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<TaskCommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // Get task
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskCommentDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TaskCommentDto>("Access denied");

        // Validate author exists
        var author = await _unitOfWork.Users.GetByIdAsync(request.AuthorId, cancellationToken);
        
        if (author == null)
            return Result.Failure<TaskCommentDto>("Author not found");

        // If replying to a comment, validate parent comment exists
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _unitOfWork.TaskComments.GetByIdAsync(request.ParentCommentId.Value, cancellationToken);
            
            if (parentComment == null)
                return Result.Failure<TaskCommentDto>("Parent comment not found");

            if (parentComment.TaskId != request.TaskId)
                return Result.Failure<TaskCommentDto>("Parent comment does not belong to this task");

            // Prevent nested replies (only 1 level allowed)
            if (parentComment.ParentCommentId.HasValue)
                return Result.Failure<TaskCommentDto>("Cannot reply to a reply. Only one level of threading is allowed");
        }

        // Create comment
        var comment = TaskComment.Create(
            request.TaskId,
            request.AuthorId,
            request.Content,
            request.ParentCommentId);

        await _unitOfWork.TaskComments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Broadcast Real-time Update
        await _notificationService.NotifyTaskCommentAddedAsync(request.TaskId, task.WorkspaceId, comment.Id, cancellationToken);

        // Map to DTO
        var dto = comment.Adapt<TaskCommentDto>();
        dto = dto with
        {
            AuthorName = author.FullName
        };

        return Result.Success(dto);
    }
}
