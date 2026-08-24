using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands.LogTime;

public class LogTimeCommandHandler : IRequestHandler<LogTimeCommand, Result<TimeEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogTimeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TimeEntryDto>> Handle(LogTimeCommand request, CancellationToken cancellationToken)
    {
        // Get task
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
            return Result.Failure<TimeEntryDto>("Task not found");

        // Validate tenant access
        if (task.TenantId != request.TenantId)
            return Result.Failure<TimeEntryDto>("Access denied");

        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return Result.Failure<TimeEntryDto>("User not found");

        if (user.TenantId != request.TenantId)
            return Result.Failure<TimeEntryDto>("User does not belong to the same tenant");

        // Validate time range
        if (request.EndTime < request.StartTime)
            return Result.Failure<TimeEntryDto>("End time cannot be before start time");

        // Create time entry
        var timeEntry = TimeEntry.CreateManual(
            request.TaskId,
            request.UserId,
            request.StartTime,
            request.EndTime,
            request.Notes);

        await _unitOfWork.TimeEntries.AddAsync(timeEntry, cancellationToken);

        // Update task actual minutes
        task.AddActualMinutes(timeEntry.DurationMinutes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = timeEntry.Adapt<TimeEntryDto>();
        dto = dto with
        {
            UserName = user.FullName
        };

        return Result.Success(dto);
    }
}
