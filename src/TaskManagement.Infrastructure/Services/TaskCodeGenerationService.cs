using TaskManagement.Domain.Services;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Infrastructure.Services;

public class TaskCodeGenerationService : ITaskCodeGenerationService
{
    public TaskCode GenerateTaskCode(string projectCode, int sequence)
    {
        return TaskCode.Create(projectCode, sequence);
    }
}
