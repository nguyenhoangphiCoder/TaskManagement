using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByCodeAsync(string code, Guid workspaceId, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task<bool> IsCodeUniqueAsync(string code, Guid workspaceId, CancellationToken cancellationToken = default);
}
