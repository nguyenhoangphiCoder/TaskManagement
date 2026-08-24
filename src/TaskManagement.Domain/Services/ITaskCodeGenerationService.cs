using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Domain.Services;

public interface ITaskCodeGenerationService
{
    TaskCode GenerateTaskCode(string projectCode, int sequence);
}
