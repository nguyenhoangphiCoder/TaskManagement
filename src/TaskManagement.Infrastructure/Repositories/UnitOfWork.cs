using Microsoft.EntityFrameworkCore.Storage;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Context;

namespace TaskManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public ITaskRepository Tasks { get; }
    public IWorkspaceRepository Workspaces { get; }
    public IProjectRepository Projects { get; }
    public IUserRepository Users { get; }
    public ITenantRepository Tenants { get; }
    public IRepository<TaskManagement.Domain.Entities.TaskStatus> TaskStatuses { get; }
    public IRepository<TaskMember> TaskMembers { get; }
    public IRepository<TaskDependency> TaskDependencies { get; }
    public IRepository<TaskChecklist> TaskChecklists { get; }
    public IRepository<TaskComment> TaskComments { get; }
    public IRepository<TaskAttachment> TaskAttachments { get; }
    public IRepository<TimeEntry> TimeEntries { get; }
    public IRepository<WorkspaceMember> WorkspaceMembers { get; }
    public IRepository<ProjectMember> ProjectMembers { get; }
    public IRepository<Tag> Tags { get; }
    public IRepository<TaskTag> TaskTags { get; }

    public UnitOfWork(
        AppDbContext context,
        ITaskRepository taskRepository,
        IWorkspaceRepository workspaceRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository)
    {
        _context = context;
        Tasks = taskRepository;
        Workspaces = workspaceRepository;
        Projects = projectRepository;
        Users = userRepository;
        Tenants = tenantRepository;
        
        // Generic repositories
        TaskStatuses = new Repository<TaskManagement.Domain.Entities.TaskStatus>(context);
        TaskMembers = new Repository<TaskMember>(context);
        TaskDependencies = new Repository<TaskDependency>(context);
        TaskChecklists = new Repository<TaskChecklist>(context);
        TaskComments = new Repository<TaskComment>(context);
        TaskAttachments = new Repository<TaskAttachment>(context);
        TimeEntries = new Repository<TimeEntry>(context);
        WorkspaceMembers = new Repository<WorkspaceMember>(context);
        ProjectMembers = new Repository<ProjectMember>(context);
        Tags = new Repository<Tag>(context);
        TaskTags = new Repository<TaskTag>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
