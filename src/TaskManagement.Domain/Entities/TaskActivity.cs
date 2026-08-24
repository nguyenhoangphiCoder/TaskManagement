using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskActivity : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string ActivityType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Task Task { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TaskActivity() { }

    public static TaskActivity Create(
        Guid taskId,
        Guid userId,
        string activityType,
        string description,
        string? oldValue = null,
        string? newValue = null)
    {
        return new TaskActivity
        {
            TaskId = taskId,
            UserId = userId,
            ActivityType = activityType,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedAt = DateTime.UtcNow
        };
    }
}
