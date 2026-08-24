using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class User : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    private User() { }

    public static User Create(Guid tenantId, string email, string fullName, string passwordHash)
    {
        return new User
        {
            TenantId = tenantId,
            Email = email,
            FullName = fullName,
            PasswordHash = passwordHash,
            Status = UserStatus.Active
        };
    }

    public void UpdateProfile(string fullName, string? avatarUrl = null)
    {
        FullName = fullName;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Activate() => Status = UserStatus.Active;
    public void Deactivate() => Status = UserStatus.Inactive;
    public void Suspend() => Status = UserStatus.Suspended;
}
