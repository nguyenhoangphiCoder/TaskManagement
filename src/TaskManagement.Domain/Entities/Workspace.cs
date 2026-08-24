using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Domain.Entities;

public class Workspace : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public WorkspaceSlug Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public WorkspaceStatus Status { get; private set; }
    public bool IsPrivate { get; private set; }

    public Tenant Tenant { get; private set; } = null!;
    public User Owner { get; private set; } = null!;

    private readonly List<WorkspaceMember> _members = new();
    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

    private readonly List<Team> _teams = new();
    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();

    private readonly List<Project> _projects = new();
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

    private readonly List<Tag> _tags = new();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private Workspace() { }

    public static Workspace Create(Guid tenantId, string name, Guid ownerId, bool isPrivate = false)
    {
        var workspace = new Workspace
        {
            TenantId = tenantId,
            Name = name,
            Slug = WorkspaceSlug.Create(name),
            OwnerId = ownerId,
            Status = WorkspaceStatus.Active,
            IsPrivate = isPrivate
        };

        workspace._members.Add(WorkspaceMember.Create(workspace.Id, ownerId, MemberRole.Owner));
        return workspace;
    }

    public void UpdateDetails(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void InviteMember(Guid userId, MemberRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already a member");

        _members.Add(WorkspaceMember.Create(Id, userId, role));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) return;
        
        if (member.Role == MemberRole.Owner)
            throw new InvalidOperationException("Cannot remove workspace owner");

        _members.Remove(member);
    }

    public void Archive() => Status = WorkspaceStatus.Archived;
    public void Activate() => Status = WorkspaceStatus.Active;
}
