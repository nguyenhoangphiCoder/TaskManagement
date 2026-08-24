using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Services;

namespace TaskManagement.Infrastructure.Services;

public class TaskDependencyValidationService : ITaskDependencyValidationService
{
    private readonly ITaskRepository _taskRepository;

    public TaskDependencyValidationService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> ValidateNoCycleAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default)
    {
        // If task depends on itself
        if (taskId == dependsOnTaskId)
            return false;

        // Check if adding this dependency would create a cycle
        var hasCycle = await _taskRepository.HasCircularDependencyAsync(taskId, dependsOnTaskId, cancellationToken);
        return !hasCycle;
    }

    public async Task<IEnumerable<Guid>> GetDependencyChainAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var chain = new List<Guid>();
        var visited = new HashSet<Guid>();

        await BuildDependencyChainRecursive(taskId, chain, visited, cancellationToken);

        return chain;
    }

    private async Task BuildDependencyChainRecursive(
        Guid taskId,
        List<Guid> chain,
        HashSet<Guid> visited,
        CancellationToken cancellationToken)
    {
        if (visited.Contains(taskId))
            return;

        visited.Add(taskId);
        chain.Add(taskId);

        var task = await _taskRepository.GetByIdWithDetailsAsync(taskId, cancellationToken);
        if (task != null)
        {
            foreach (var dependency in task.Dependencies)
            {
                await BuildDependencyChainRecursive(
                    dependency.DependsOnTaskId,
                    chain,
                    visited,
                    cancellationToken);
            }
        }
    }
}
