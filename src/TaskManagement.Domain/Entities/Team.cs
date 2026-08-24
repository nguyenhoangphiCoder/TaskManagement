using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class Team : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid LeaderId { get; private set; }

    public Workspace Workspace { get; private set; } = null!;
    public User Leader { get; private set; } = null!;

    private readonly List<TeamMember> _members = new();
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

    private Team() { }

    public static Team Create(Guid tenantId, Guid workspaceId, string name, Guid leaderId)
    {
        var team = new Team
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            Name = name,
            LeaderId = leaderId
        };

        team._members.Add(TeamMember.Create(team.Id, leaderId));
        return team;
    }

    public void AddMember(Guid userId)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already in team");

        _members.Add(TeamMember.Create(Id, userId));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null) _members.Remove(member);
    }

    public void ChangeLeader(Guid newLeaderId)
    {
        if (!_members.Any(m => m.UserId == newLeaderId))
            throw new InvalidOperationException("New leader must be a team member");

        LeaderId = newLeaderId;
    }
}
