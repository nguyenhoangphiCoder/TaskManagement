using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class TaskComment : BaseAuditableEntity
{
    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public string Content { get; private set; } = string.Empty;

    public Task Task { get; private set; } = null!;
    public User Author { get; private set; } = null!;
    public TaskComment? ParentComment { get; private set; }

    private readonly List<TaskComment> _replies = new();
    public IReadOnlyCollection<TaskComment> Replies => _replies.AsReadOnly();

    private TaskComment() { }

    public static TaskComment Create(Guid taskId, Guid authorId, string content, Guid? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        return new TaskComment
        {
            TaskId = taskId,
            AuthorId = authorId,
            ParentCommentId = parentCommentId,
            Content = content
        };
    }

    public void Update(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        Content = content;
    }
}
