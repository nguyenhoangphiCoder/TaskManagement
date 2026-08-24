using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskDependency : BaseEntity
{
    public Guid TaskId { get; private set; }
    public Guid DependsOnTaskId { get; private set; }
    public DependencyType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Task Task { get; private set; } = null!;
    public Task DependsOnTask { get; private set; } = null!;

    private TaskDependency() { }

    public static TaskDependency Create(
        Guid taskId,
        Guid dependsOnTaskId,
        DependencyType type = DependencyType.FinishToStart)
    {
        if (taskId == dependsOnTaskId)
            throw new InvalidOperationException("Task cannot depend on itself");

        return new TaskDependency
        {
            TaskId = taskId,
            DependsOnTaskId = dependsOnTaskId,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };
    }
}
