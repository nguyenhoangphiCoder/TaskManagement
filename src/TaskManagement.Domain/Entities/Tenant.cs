using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public SubscriptionPlan Plan { get; private set; }
    public int MaxUsers { get; private set; }
    public int MaxWorkspaces { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? SubscriptionExpiresAt { get; private set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private readonly List<Workspace> _workspaces = new();
    public IReadOnlyCollection<Workspace> Workspaces => _workspaces.AsReadOnly();

    private Tenant() { }

    public static Tenant Create(string name, SubscriptionPlan plan = SubscriptionPlan.Free)
    {
        var tenant = new Tenant
        {
            Name = name,
            Plan = plan,
            IsActive = true
        };

        tenant.SetPlanLimits(plan);
        return tenant;
    }

    public void UpdatePlan(SubscriptionPlan newPlan, DateTime? expiresAt = null)
    {
        Plan = newPlan;
        SubscriptionExpiresAt = expiresAt;
        SetPlanLimits(newPlan);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void SetPlanLimits(SubscriptionPlan plan)
    {
        (MaxUsers, MaxWorkspaces) = plan switch
        {
            SubscriptionPlan.Free => (5, 2),
            SubscriptionPlan.Basic => (15, 5),
            SubscriptionPlan.Professional => (50, 20),
            SubscriptionPlan.Enterprise => (int.MaxValue, int.MaxValue),
            _ => (5, 2)
        };
    }
}
