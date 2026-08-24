using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
}
