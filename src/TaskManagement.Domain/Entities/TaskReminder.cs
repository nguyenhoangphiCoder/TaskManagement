using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskReminder : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime RemindAt { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentAt { get; private set; }

    public Task Task { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TaskReminder() { }

    public static TaskReminder Create(Guid taskId, Guid userId, DateTime remindAt)
    {
        if (remindAt <= DateTime.UtcNow)
            throw new ArgumentException("Reminder time must be in the future", nameof(remindAt));

        return new TaskReminder
        {
            TaskId = taskId,
            UserId = userId,
            RemindAt = remindAt,
            IsSent = false
        };
    }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
    }
}
