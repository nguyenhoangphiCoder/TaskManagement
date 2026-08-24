using System;

namespace TaskManagement.Application.DTOs.Tags;

public record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}
