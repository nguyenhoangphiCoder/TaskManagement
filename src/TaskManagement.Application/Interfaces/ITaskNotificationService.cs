using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces;

public interface ITaskNotificationService
{
    Task NotifyTaskUpdatedAsync(Guid taskId, Guid workspaceId, CancellationToken cancellationToken = default);
    Task NotifyTaskStatusChangedAsync(Guid taskId, Guid workspaceId, Guid newStatusId, CancellationToken cancellationToken = default);
    Task NotifyTaskCommentAddedAsync(Guid taskId, Guid workspaceId, Guid commentId, CancellationToken cancellationToken = default);
}
