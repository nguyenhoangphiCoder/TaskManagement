using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class RecurringTaskTemplate : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Guid ProjectId { get; private set; }
    
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid StatusId { get; private set; }
    
    public string RecurrenceRule { get; private set; } = string.Empty; // iCalendar RRULE format
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastGeneratedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public Workspace Workspace { get; private set; } = null!;

    private RecurringTaskTemplate() { }

    public static RecurringTaskTemplate Create(
        Guid tenantId,
        Guid workspaceId,
        Guid projectId,
        string title,
        Guid statusId,
        string recurrenceRule,
        DateTime startDate)
    {
        return new RecurringTaskTemplate
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            Title = title,
            StatusId = statusId,
            Priority = TaskPriority.Medium,
            RecurrenceRule = recurrenceRule,
            StartDate = startDate,
            IsActive = true
        };
    }

    public void UpdateRecurrence(string recurrenceRule, DateTime startDate, DateTime? endDate = null)
    {
        RecurrenceRule = recurrenceRule;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void MarkGenerated()
    {
        LastGeneratedAt = DateTime.UtcNow;
    }
}
