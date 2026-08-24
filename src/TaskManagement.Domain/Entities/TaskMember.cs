using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskMember : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public TaskMemberType MemberType { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public Task Task { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TaskMember() { }

    public static TaskMember Create(Guid taskId, Guid userId, TaskMemberType memberType)
    {
        return new TaskMember
        {
            TaskId = taskId,
            UserId = userId,
            MemberType = memberType,
            AssignedAt = DateTime.UtcNow
        };
    }

    public void ChangeType(TaskMemberType newType)
    {
        MemberType = newType;
    }
}
