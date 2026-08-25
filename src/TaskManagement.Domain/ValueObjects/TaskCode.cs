namespace TaskManagement.Domain.ValueObjects;

public sealed class TaskCode : IEquatable<TaskCode>
{
    public string Value { get; private set; } = string.Empty;

    public TaskCode(string value)
    {
        Value = value ?? string.Empty;
    }

    private TaskCode() { }

    public static TaskCode Create(string projectCode, int sequence)
    {
        if (string.IsNullOrWhiteSpace(projectCode))
            throw new ArgumentException("Project code cannot be empty", nameof(projectCode));
        
        if (sequence <= 0)
            throw new ArgumentException("Sequence must be greater than 0", nameof(sequence));

        return new TaskCode($"{projectCode}-{sequence}");
    }

    public static TaskCode FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new TaskCode(string.Empty);

        return new TaskCode(value);
    }

    public bool Equals(TaskCode? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is TaskCode other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(TaskCode? left, TaskCode? right) => 
        left?.Equals(right) ?? right is null;
    public static bool operator !=(TaskCode? left, TaskCode? right) => !(left == right);
}
