using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDetailDto>>;
