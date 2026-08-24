using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Domain.Entities;

public class Task : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? ParentTaskId { get; private set; }
    
    public TaskCode Code { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid StatusId { get; private set; }
    
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public int EstimatedMinutes { get; private set; }
    public int ActualMinutes { get; private set; }
    
    public double SortOrder { get; private set; }
    public int SubtaskLevel { get; private set; }

    public Project Project { get; private set; } = null!;
    public Workspace Workspace { get; private set; } = null!;
    public TaskStatus Status { get; private set; } = null!;
    public Task? ParentTask { get; private set; }

    private readonly List<Task> _subtasks = new();
    public IReadOnlyCollection<Task> Subtasks => _subtasks.AsReadOnly();

    private readonly List<TaskMember> _members = new();
    public IReadOnlyCollection<TaskMember> Members => _members.AsReadOnly();

    private readonly List<TaskDependency> _dependencies = new();
    public IReadOnlyCollection<TaskDependency> Dependencies => _dependencies.AsReadOnly();

    private readonly List<TaskTag> _tags = new();
    public IReadOnlyCollection<TaskTag> Tags => _tags.AsReadOnly();

    private readonly List<TaskChecklist> _checklists = new();
    public IReadOnlyCollection<TaskChecklist> Checklists => _checklists.AsReadOnly();

    private readonly List<TaskComment> _comments = new();
    public IReadOnlyCollection<TaskComment> Comments => _comments.AsReadOnly();

    private readonly List<TaskAttachment> _attachments = new();
    public IReadOnlyCollection<TaskAttachment> Attachments => _attachments.AsReadOnly();

    private readonly List<TimeEntry> _timeEntries = new();
    public IReadOnlyCollection<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();

    private Task() { }

    public static Task Create(
        Guid tenantId,
        Guid workspaceId,
        Guid projectId,
        string title,
        TaskCode code,
        Guid statusId,
        TaskPriority priority = TaskPriority.Medium)
    {
        return new Task
        {
            TenantId = tenantId,
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            Title = title,
            Code = code,
            StatusId = statusId,
            Priority = priority,
            SubtaskLevel = 0,
            SortOrder = DateTime.UtcNow.Ticks
        };
    }

    public void UpdateDetails(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }

    public void ChangePriority(TaskPriority newPriority)
    {
        Priority = newPriority;
    }

    public void ChangeStatus(Guid newStatusId)
    {
        StatusId = newStatusId;
    }

    public void SetParentTask(Guid parentTaskId, int parentSubtaskLevel)
    {
        if (parentTaskId == Id)
            throw new ArgumentException("Task cannot be its own parent", nameof(parentTaskId));

        ParentTaskId = parentTaskId;
        SubtaskLevel = parentSubtaskLevel + 1;
    }

    public void SetDates(DateTime? startDate, DateTime? dueDate)
    {
        if (startDate.HasValue && dueDate.HasValue && dueDate.Value < startDate.Value)
            throw new ArgumentException("Due date cannot be before start date");

        StartDate = startDate;
        DueDate = dueDate;
    }

    public void SetEstimate(int minutes)
    {
        if (minutes < 0)
            throw new ArgumentException("Estimated minutes cannot be negative");

        EstimatedMinutes = minutes;
    }

    public void AddActualMinutes(int minutes)
    {
        if (minutes < 0)
            throw new ArgumentException("Actual minutes cannot be negative");

        ActualMinutes += minutes;
    }

    public void AssignMember(Guid userId, TaskMemberType type)
    {
        if (_members.Any(m => m.UserId == userId && m.MemberType == type))
            return;

        _members.Add(TaskMember.Create(Id, userId, type));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null) _members.Remove(member);
    }

    public bool IsOverdue() => DueDate.HasValue && DateTime.UtcNow > DueDate.Value;

    public void UpdateSortOrder(double newSortOrder)
    {
        SortOrder = newSortOrder;
    }

    public void AddComment(Guid authorId, string content, Guid? parentCommentId = null)
    {
        _comments.Add(TaskComment.Create(Id, authorId, content, parentCommentId));
    }

    public void AddChecklist(string title, int position)
    {
        _checklists.Add(TaskChecklist.Create(Id, title, position));
    }

    public void AddTag(Guid tagId)
    {
        if (!_tags.Any(t => t.TagId == tagId))
        {
            _tags.Add(TaskTag.Create(Id, tagId));
        }
    }
}
