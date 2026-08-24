using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.AddComment;

public record AddCommentCommand : IRequest<Result<TaskCommentDto>>
{
    public Guid TaskId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
    public Guid TenantId { get; init; }
    public Guid AuthorId { get; init; }
}
