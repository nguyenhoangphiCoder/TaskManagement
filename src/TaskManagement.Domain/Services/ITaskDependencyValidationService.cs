namespace TaskManagement.Domain.Services;

public interface ITaskDependencyValidationService
{
    Task<bool> ValidateNoCycleAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetDependencyChainAsync(Guid taskId, CancellationToken cancellationToken = default);
}
