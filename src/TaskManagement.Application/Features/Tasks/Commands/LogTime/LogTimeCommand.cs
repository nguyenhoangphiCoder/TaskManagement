using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.LogTime;

public record LogTimeCommand : IRequest<Result<TimeEntryDto>>
{
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string? Notes { get; init; }
    public Guid TenantId { get; init; }
    public Guid CreatedBy { get; init; }
}
