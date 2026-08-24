using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}
