using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Context;

namespace TaskManagement.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Project?> GetByCodeAsync(string code, Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Code == code && p.WorkspaceId == workspaceId, cancellationToken);
    }

    public async Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Workspace)
            .Include(p => p.Members)
                .ThenInclude(pm => pm.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.WorkspaceId == workspaceId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return !await _dbSet
            .AnyAsync(p => p.Code == code && p.WorkspaceId == workspaceId, cancellationToken);
    }
}
