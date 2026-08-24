using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand : IRequest<Result<bool>>
{
    public Guid TaskId { get; init; }
    public Guid TenantId { get; init; }
    public Guid DeletedBy { get; init; }
}
