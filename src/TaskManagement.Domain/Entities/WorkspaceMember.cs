using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class WorkspaceMember : BaseEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Workspace Workspace { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private WorkspaceMember() { }

    public static WorkspaceMember Create(Guid workspaceId, Guid userId, MemberRole role)
    {
        return new WorkspaceMember
        {
            WorkspaceId = workspaceId,
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
