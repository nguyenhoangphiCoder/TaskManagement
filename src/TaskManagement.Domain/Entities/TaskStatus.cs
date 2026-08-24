using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskStatus : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TaskStatusType Type { get; private set; }
    public string Color { get; private set; } = "#gray";
    public int Position { get; private set; }
    public bool IsDefault { get; private set; }

    public Workspace Workspace { get; private set; } = null!;

    private TaskStatus() { }

    public static TaskStatus Create(
        Guid tenantId,
        Guid workspaceId,
        string name,
        TaskStatusType type,
        int position,
        string color = "#gray")
    {
        return new TaskStatus
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            Name = name,
            Type = type,
            Color = color,
            Position = position,
            IsDefault = false
        };
    }

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public void SetAsDefault() => IsDefault = true;
}
