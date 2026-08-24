using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using System.Collections.Generic;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskComments;

public record GetTaskCommentsQuery : IRequest<Result<List<TaskCommentDto>>>
{
    public Guid TaskId { get; init; }
}
