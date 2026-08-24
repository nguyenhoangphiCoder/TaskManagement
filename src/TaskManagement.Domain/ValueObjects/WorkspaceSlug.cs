using System.Text.RegularExpressions;

namespace TaskManagement.Domain.ValueObjects;

public sealed class WorkspaceSlug : IEquatable<WorkspaceSlug>
{
    public string Value { get; private set; }

    private WorkspaceSlug(string value)
    {
        Value = value;
    }

    public static WorkspaceSlug Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workspace name cannot be empty", nameof(name));

        var slug = GenerateSlug(name);
        return new WorkspaceSlug(slug);
    }

    public static WorkspaceSlug FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Slug cannot be empty", nameof(value));

        if (!IsValidSlug(value))
            throw new ArgumentException("Invalid slug format", nameof(value));

        return new WorkspaceSlug(value);
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        return slug;
    }

    private static bool IsValidSlug(string value)
    {
        return Regex.IsMatch(value, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
    }

    public bool Equals(WorkspaceSlug? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is WorkspaceSlug other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(WorkspaceSlug? left, WorkspaceSlug? right) => 
        left?.Equals(right) ?? right is null;
    public static bool operator !=(WorkspaceSlug? left, WorkspaceSlug? right) => !(left == right);
}
