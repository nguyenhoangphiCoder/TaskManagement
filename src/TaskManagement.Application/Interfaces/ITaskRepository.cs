using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.Interfaces;

public interface ITaskRepository : IRepository<Domain.Entities.Task>
{
    Task<Domain.Entities.Task?> GetByCodeAsync(TaskCode code, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Task?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Task>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Task>> GetOverdueTasksAsync(DateTime currentDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Task>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Task>> GetTasksWithDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<int> GetNextTaskSequenceAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> HasCircularDependencyAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default);
}
