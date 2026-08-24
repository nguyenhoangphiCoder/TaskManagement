using System;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Workspaces;

public record WorkspaceMemberDto
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public MemberRole Role { get; init; }
    public DateTime JoinedAt { get; init; }
}
