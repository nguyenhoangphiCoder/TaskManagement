using System;

namespace TaskManagement.Application.DTOs.Tasks;

public record TaskStatusDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public int Position { get; init; }
    public bool IsDefault { get; init; }
}
