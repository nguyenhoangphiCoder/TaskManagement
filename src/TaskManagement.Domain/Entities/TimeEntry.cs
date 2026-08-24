using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? Notes { get; private set; }
    public bool IsRunning { get; private set; }

    public Task Task { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TimeEntry() { }

    public static TimeEntry Start(Guid taskId, Guid userId, string? notes = null)
    {
        return new TimeEntry
        {
            TaskId = taskId,
            UserId = userId,
            StartTime = DateTime.UtcNow,
            Notes = notes,
            IsRunning = true
        };
    }

    public static TimeEntry CreateManual(
        Guid taskId,
        Guid userId,
        DateTime startTime,
        DateTime endTime,
        string? notes = null)
    {
        if (endTime < startTime)
            throw new ArgumentException("End time cannot be before start time");

        var duration = (int)(endTime - startTime).TotalMinutes;

        return new TimeEntry
        {
            TaskId = taskId,
            UserId = userId,
            StartTime = startTime,
            EndTime = endTime,
            DurationMinutes = duration,
            Notes = notes,
            IsRunning = false
        };
    }

    public void Stop()
    {
        if (!IsRunning)
            throw new InvalidOperationException("Time entry is not running");

        EndTime = DateTime.UtcNow;
        DurationMinutes = (int)(EndTime.Value - StartTime).TotalMinutes;
        IsRunning = false;
    }

    public void Update(DateTime startTime, DateTime endTime, string? notes = null)
    {
        if (IsRunning)
            throw new InvalidOperationException("Cannot update running time entry");

        if (endTime < startTime)
            throw new ArgumentException("End time cannot be before start time");

        StartTime = startTime;
        EndTime = endTime;
        DurationMinutes = (int)(endTime - startTime).TotalMinutes;
        Notes = notes;
    }
}
