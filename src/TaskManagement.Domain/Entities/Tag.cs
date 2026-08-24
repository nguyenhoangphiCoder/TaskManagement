using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class Tag : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = "#gray";

    public Workspace Workspace { get; private set; } = null!;

    private Tag() { }

    public static Tag Create(Guid tenantId, Guid workspaceId, string name, string color = "#gray")
    {
        return new Tag
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            Name = name,
            Color = color
        };
    }

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
    }
}
