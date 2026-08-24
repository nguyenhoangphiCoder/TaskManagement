namespace TaskManagement.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITaskRepository Tasks { get; }
    IWorkspaceRepository Workspaces { get; }
    IProjectRepository Projects { get; }
    IUserRepository Users { get; }
    ITenantRepository Tenants { get; }
    IRepository<Domain.Entities.TaskStatus> TaskStatuses { get; }
    IRepository<Domain.Entities.TaskMember> TaskMembers { get; }
    IRepository<Domain.Entities.TaskDependency> TaskDependencies { get; }
    IRepository<Domain.Entities.TaskChecklist> TaskChecklists { get; }
    IRepository<Domain.Entities.TaskComment> TaskComments { get; }
    IRepository<Domain.Entities.TaskAttachment> TaskAttachments { get; }
    IRepository<Domain.Entities.TimeEntry> TimeEntries { get; }
    IRepository<Domain.Entities.WorkspaceMember> WorkspaceMembers { get; }
    IRepository<Domain.Entities.ProjectMember> ProjectMembers { get; }
    IRepository<Domain.Entities.Tag> Tags { get; }
    IRepository<Domain.Entities.TaskTag> TaskTags { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
