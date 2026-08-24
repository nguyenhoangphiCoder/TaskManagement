using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.AddChecklistItem;

public record AddChecklistItemCommand : IRequest<Result<TaskChecklistDto>>
{
    public Guid TaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Position { get; init; }
    public Guid TenantId { get; init; }
    public Guid CreatedBy { get; init; }
}
