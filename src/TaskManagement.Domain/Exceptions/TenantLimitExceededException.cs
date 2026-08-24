namespace TaskManagement.Domain.Exceptions;

public class TenantLimitExceededException : DomainException
{
    public TenantLimitExceededException(string resourceType, int limit)
        : base($"Tenant limit exceeded for {resourceType}. Maximum allowed: {limit}")
    {
    }
}
