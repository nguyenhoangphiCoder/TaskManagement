namespace TaskManagement.Application.DTOs.Tasks;

public record CreateTaskChecklistDto
{
    public string Title { get; init; } = string.Empty;
    public int Position { get; init; }
}
