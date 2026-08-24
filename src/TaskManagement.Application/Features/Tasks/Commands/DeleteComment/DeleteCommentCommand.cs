using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteComment;

public record DeleteCommentCommand : IRequest<Result<bool>>
{
    public Guid CommentId { get; init; }
    public Guid TaskId { get; init; }
    public Guid TenantId { get; init; }
    public Guid DeletedBy { get; init; }
}
