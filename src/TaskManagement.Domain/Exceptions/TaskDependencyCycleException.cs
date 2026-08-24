namespace TaskManagement.Domain.Exceptions;

public class TaskDependencyCycleException : DomainException
{
    public TaskDependencyCycleException(Guid taskId, Guid dependsOnTaskId)
        : base($"Cannot create dependency: Adding task {dependsOnTaskId} as dependency of task {taskId} would create a circular dependency.")
    {
    }
}
