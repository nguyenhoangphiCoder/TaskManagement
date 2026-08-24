using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.Interfaces;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<Workspace?> GetBySlugAsync(WorkspaceSlug slug, CancellationToken cancellationToken = default);
    Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsSlugUniqueAsync(WorkspaceSlug slug, Guid tenantId, CancellationToken cancellationToken = default);
}
