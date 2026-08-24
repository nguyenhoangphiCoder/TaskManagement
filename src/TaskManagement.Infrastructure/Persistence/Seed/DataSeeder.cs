using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;
using TaskManagement.Infrastructure.Persistence.Context;

namespace TaskManagement.Infrastructure.Persistence.Seed;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task SeedAsync()
    {
        if (await _context.Tenants.AnyAsync())
            return; // Already seeded

        // 1. Create Tenant
        var tenant = Tenant.Create("Demo Company", SubscriptionPlan.Professional);
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        // 2. Create User
        var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("admin123");
        var user = User.Create(tenant.Id, "admin@demo.com", "Admin User", passwordHash);
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        // 3. Create Workspace
        var workspace = Workspace.Create(tenant.Id, "Main Workspace", user.Id, false);
        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();

        // 4. Create Tags
        var tagFrontend = Tag.Create(tenant.Id, workspace.Id, "Frontend", "#3B82F6");
        var tagBackend = Tag.Create(tenant.Id, workspace.Id, "Backend", "#10B981");
        var tagBug = Tag.Create(tenant.Id, workspace.Id, "Bug", "#EF4444");
        
        _context.Tags.AddRange(tagFrontend, tagBackend, tagBug);
        await _context.SaveChangesAsync();

        // 5. Create Project
        var project = Project.Create(tenant.Id, workspace.Id, "Project Alpha", "PRJ", true);
        project.Activate(); // Set to Active
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // 6. Create Task Statuses
        var statusTodo = Domain.Entities.TaskStatus.Create(
            tenant.Id,
            workspace.Id,
            "To Do", 
            TaskStatusType.NotStarted, 
            0,
            "#94A3B8"
        );
        var statusInProgress = Domain.Entities.TaskStatus.Create(
            tenant.Id,
            workspace.Id,
            "In Progress", 
            TaskStatusType.InProgress, 
            1,
            "#3B82F6"
        );
        var statusDone = Domain.Entities.TaskStatus.Create(
            tenant.Id,
            workspace.Id,
            "Done", 
            TaskStatusType.Completed, 
            2,
            "#10B981"
        );
        
        _context.TaskStatuses.AddRange(statusTodo, statusInProgress, statusDone);
        await _context.SaveChangesAsync();

        // 7. Create Tasks
        var task1 = Domain.Entities.Task.Create(
            tenant.Id,
            workspace.Id,
            project.Id,
            "Implement user authentication",
            TaskCode.Create("PRJ", 1),
            statusTodo.Id,
            TaskPriority.High
        );
        task1.SetDates(DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        var task2 = Domain.Entities.Task.Create(
            tenant.Id,
            workspace.Id,
            project.Id,
            "Design database schema",
            TaskCode.Create("PRJ", 2),
            statusInProgress.Id,
            TaskPriority.High
        );
        task2.SetDates(DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        var task3 = Domain.Entities.Task.Create(
            tenant.Id,
            workspace.Id,
            project.Id,
            "Setup CI/CD pipeline",
            TaskCode.Create("PRJ", 3),
            statusTodo.Id,
            TaskPriority.Medium
        );
        task3.SetDates(DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

        _context.Tasks.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();
    }
}
