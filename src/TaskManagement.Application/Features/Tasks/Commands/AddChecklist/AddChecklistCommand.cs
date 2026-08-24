using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using System;

namespace TaskManagement.Application.Features.Tasks.Commands.AddChecklist;

public record AddChecklistCommand : IRequest<Result<TaskChecklistDto>>
{
    public Guid TaskId { get; init; }
    public CreateTaskChecklistDto Checklist { get; init; } = null!;
    public Guid TenantId { get; init; }
}
