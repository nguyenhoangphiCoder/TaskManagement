namespace TaskManagement.Domain.ValueObjects;

public sealed class DateTimeRange : IEquatable<DateTimeRange>
{
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public DateTimeRange(DateTime startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    private DateTimeRange() { }

    public static DateTimeRange Create(DateTime startDate, DateTime? endDate = null)
    {
        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date cannot be before start date");

        return new DateTimeRange(startDate, endDate);
    }

    public int GetDurationInDays()
    {
        if (!EndDate.HasValue) return 0;
        return (EndDate.Value - StartDate).Days;
    }

    public bool IsOverdue(DateTime currentDate)
    {
        if (!EndDate.HasValue) return false;
        return currentDate > EndDate.Value;
    }

    public bool Contains(DateTime date)
    {
        if (!EndDate.HasValue) return date >= StartDate;
        return date >= StartDate && date <= EndDate.Value;
    }

    public bool Equals(DateTimeRange? other)
    {
        if (other is null) return false;
        return StartDate == other.StartDate && EndDate == other.EndDate;
    }

    public override bool Equals(object? obj) => obj is DateTimeRange other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StartDate, EndDate);

    public static bool operator ==(DateTimeRange? left, DateTimeRange? right) => 
        left?.Equals(right) ?? right is null;
    public static bool operator !=(DateTimeRange? left, DateTimeRange? right) => !(left == right);
}
