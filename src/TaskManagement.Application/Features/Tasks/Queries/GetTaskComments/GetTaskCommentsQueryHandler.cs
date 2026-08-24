using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskComments;

public class GetTaskCommentsQueryHandler : IRequestHandler<GetTaskCommentsQuery, Result<List<TaskCommentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskCommentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<TaskCommentDto>>> Handle(GetTaskCommentsQuery request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);
        if (task == null)
            return Result.Failure<List<TaskCommentDto>>("Task not found");

        var commentsDto = task.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new TaskCommentDto
        {
            Id = c.Id,
            TaskId = c.TaskId,
            AuthorId = c.AuthorId,
            AuthorName = c.Author?.FullName ?? "Unknown",
            ParentCommentId = c.ParentCommentId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        }).ToList();

        return Result.Success(commentsDto);
    }
}
