using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ValueObjects;
using TaskManagement.Infrastructure.Persistence.Context;

namespace TaskManagement.Infrastructure.Repositories;

public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Workspace?> GetBySlugAsync(WorkspaceSlug slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.Slug == slug, cancellationToken);
    }

    public async Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .Include(w => w.Members)
                .ThenInclude(m => m.User)
            .Include(w => w.Teams)
            .Include(w => w.Tags)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .Where(w => w.OwnerId == userId || w.Members.Any(m => m.UserId == userId))
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSlugUniqueAsync(WorkspaceSlug slug, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return !await _dbSet
            .AnyAsync(w => w.Slug == slug && w.TenantId == tenantId, cancellationToken);
    }
}
