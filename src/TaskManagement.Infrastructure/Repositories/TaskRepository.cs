using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.ValueObjects;
using TaskManagement.Infrastructure.Persistence.Context;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : Repository<Domain.Entities.Task>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Task?> GetByCodeAsync(TaskCode code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public async Task<Domain.Entities.Task?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.Members)
                .ThenInclude(tm => tm.User)
            .Include(t => t.Dependencies)
                .ThenInclude(td => td.DependsOnTask)
            .Include(t => t.Checklists)
            .Include(t => t.Comments)
                .ThenInclude(tc => tc.Author)
            .Include(t => t.Attachments)
            .Include(t => t.TimeEntries)
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Task>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Status)
            .Include(t => t.Members)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Task>> GetOverdueTasksAsync(DateTime currentDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Status)
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < currentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Task>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Where(t => t.Members.Any(tm => tm.UserId == userId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Task>> GetTasksWithDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Dependencies)
                .ThenInclude(td => td.DependsOnTask)
            .Where(t => t.Id == taskId || t.Dependencies.Any(d => d.DependsOnTaskId == taskId))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextTaskSequenceAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var maxSequence = await _dbSet
            .Where(t => t.ProjectId == projectId)
            .CountAsync(cancellationToken);

        return maxSequence + 1;
    }

    public async Task<bool> HasCircularDependencyAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default)
    {
        // Simple check: if dependsOnTask depends on taskId (directly or indirectly)
        var visited = new HashSet<Guid>();
        return await CheckCircularDependencyRecursive(dependsOnTaskId, taskId, visited, cancellationToken);
    }

    private async Task<bool> CheckCircularDependencyRecursive(
        Guid currentTaskId,
        Guid targetTaskId,
        HashSet<Guid> visited,
        CancellationToken cancellationToken)
    {
        if (currentTaskId == targetTaskId)
            return true;

        if (visited.Contains(currentTaskId))
            return false;

        visited.Add(currentTaskId);

        var dependencies = await _context.TaskDependencies
            .Where(td => td.TaskId == currentTaskId)
            .Select(td => td.DependsOnTaskId)
            .ToListAsync(cancellationToken);

        foreach (var depId in dependencies)
        {
            if (await CheckCircularDependencyRecursive(depId, targetTaskId, visited, cancellationToken))
                return true;
        }

        return false;
    }
}
