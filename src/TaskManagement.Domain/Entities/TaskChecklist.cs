using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskChecklist : BaseEntity
{
    public Guid TaskId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }
    public int Position { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Task Task { get; private set; } = null!;

    private TaskChecklist() { }

    public static TaskChecklist Create(Guid taskId, string title, int position)
    {
        return new TaskChecklist
        {
            TaskId = taskId,
            Title = title,
            IsCompleted = false,
            Position = position,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Toggle()
    {
        IsCompleted = !IsCompleted;
    }

    public void Complete() => IsCompleted = true;
    public void Uncomplete() => IsCompleted = false;

    public void Update(string title)
    {
        Title = title;
    }
}
