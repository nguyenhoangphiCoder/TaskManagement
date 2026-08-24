using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Domain.Entities;

public class Project : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateTimeRange? Timeline { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public bool IsPublicToWorkspace { get; private set; }

    public Workspace Workspace { get; private set; } = null!;

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

    private readonly List<Task> _tasks = new();
    public IReadOnlyCollection<Task> Tasks => _tasks.AsReadOnly();

    private Project() { }

    public static Project Create(Guid tenantId, Guid workspaceId, string name, string code, bool isPublic = true)
    {
        return new Project
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            Name = name,
            Code = code.ToUpperInvariant(),
            Status = ProjectStatus.Draft,
            IsPublicToWorkspace = isPublic,
            ProgressPercentage = 0
        };
    }

    public void UpdateDetails(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void SetTimeline(DateTime startDate, DateTime? endDate = null)
    {
        Timeline = DateTimeRange.Create(startDate, endDate);
    }

    public void UpdateProgress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        ProgressPercentage = percentage;
    }

    public void Activate() => Status = ProjectStatus.Active;
    public void Complete() => Status = ProjectStatus.Completed;
    public void Archive() => Status = ProjectStatus.Archived;
    public void PutOnHold() => Status = ProjectStatus.OnHold;

    public void AddMember(Guid userId, MemberRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already a project member");

        _members.Add(ProjectMember.Create(Id, userId, role));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null) _members.Remove(member);
    }
}
