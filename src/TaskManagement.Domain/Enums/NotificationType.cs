namespace TaskManagement.Domain.Enums;

public enum NotificationType
{
    TaskAssigned = 1,
    TaskStatusChanged = 2,
    TaskCommentAdded = 3,
    TaskMentioned = 4,
    TaskDueDateReminder = 5,
    WorkspaceInvite = 6,
    ProjectInvite = 7
}
