using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.API.Hubs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Services;

public class TaskNotificationService : ITaskNotificationService
{
    private readonly IHubContext<TaskHub> _hubContext;

    public TaskNotificationService(IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTaskUpdatedAsync(Guid taskId, Guid workspaceId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync("TaskUpdated", taskId, cancellationToken);
    }

    public async Task NotifyTaskStatusChangedAsync(Guid taskId, Guid workspaceId, Guid newStatusId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync("TaskStatusChanged", new { TaskId = taskId, NewStatusId = newStatusId }, cancellationToken);
    }

    public async Task NotifyTaskCommentAddedAsync(Guid taskId, Guid workspaceId, Guid commentId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync("TaskCommentAdded", new { TaskId = taskId, CommentId = commentId }, cancellationToken);
    }
}
