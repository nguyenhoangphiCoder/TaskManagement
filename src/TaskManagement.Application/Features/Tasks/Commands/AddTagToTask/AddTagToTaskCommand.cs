using MediatR;
using TaskManagement.Application.Common;
using System;

namespace TaskManagement.Application.Features.Tasks.Commands.AddTagToTask;

public record AddTagToTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; init; }
    public Guid TagId { get; init; }
    public Guid TenantId { get; init; }
}
