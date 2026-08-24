using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskTag : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid TagId { get; private set; }

    public Task Task { get; private set; } = null!;
    public Tag Tag { get; private set; } = null!;

    private TaskTag() { }

    public static TaskTag Create(Guid taskId, Guid tagId)
    {
        return new TaskTag
        {
            TaskId = taskId,
            TagId = tagId
        };
    }
}
