using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Team Team { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TeamMember() { }

    public static TeamMember Create(Guid teamId, Guid userId)
    {
        return new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };
    }
}
