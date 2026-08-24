using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class ProjectMember : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private ProjectMember() { }

    public static ProjectMember Create(Guid projectId, Guid userId, MemberRole role)
    {
        return new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void UpdateRole(MemberRole newRole)
    {
        Role = newRole;
    }
}
