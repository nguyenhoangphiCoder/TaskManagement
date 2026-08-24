using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default);
}
