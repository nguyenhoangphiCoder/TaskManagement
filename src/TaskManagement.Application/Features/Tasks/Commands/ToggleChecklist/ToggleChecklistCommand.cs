using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.ToggleChecklist;

public record ToggleChecklistCommand : IRequest<Result<TaskChecklistDto>>
{
    public Guid ChecklistItemId { get; init; }
    public Guid TaskId { get; init; }
    public Guid TenantId { get; init; }
    public Guid ModifiedBy { get; init; }
}
