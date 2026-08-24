using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tenant & Users
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> AppUsers => Set<User>();

    // Workspaces
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Tag> Tags => Set<Tag>();

    // Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    // Tasks
    public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();
    public DbSet<Domain.Entities.TaskStatus> TaskStatuses => Set<Domain.Entities.TaskStatus>();
    public DbSet<TaskMember> TaskMembers => Set<TaskMember>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
    public DbSet<TaskChecklist> TaskChecklists => Set<TaskChecklist>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<TaskActivity> TaskActivities => Set<TaskActivity>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<TaskReminder> TaskReminders => Set<TaskReminder>();

    // Recurring Tasks
    public DbSet<RecurringTaskTemplate> RecurringTaskTemplates => Set<RecurringTaskTemplate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filter for multi-tenancy
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetGlobalTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);
                
                method?.Invoke(null, new object[] { builder });
            }

            // Soft delete filter
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);
                
                method?.Invoke(null, new object[] { builder });
            }
        }
    }

    private static void SetGlobalTenantFilter<TEntity>(ModelBuilder builder) where TEntity : class, ITenantEntity
    {
        // Placeholder - will be implemented with tenant context service
        // builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == currentTenantId);
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : BaseAuditableEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    // entry.Entity.CreatedBy = _currentUserService.UserId; // TODO: Implement
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // entry.Entity.UpdatedBy = _currentUserService.UserId; // TODO: Implement
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    // entry.Entity.DeletedBy = _currentUserService.UserId; // TODO: Implement
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
