using TaskManagement.Domain.Services;

namespace TaskManagement.Infrastructure.Services;

public class ProjectProgressCalculationService : IProjectProgressCalculationService
{
    public decimal CalculateProgress(int totalTasks, int completedTasks)
    {
        if (totalTasks == 0)
            return 0;

        return Math.Round((decimal)completedTasks / totalTasks * 100, 2);
    }

    public decimal CalculateProgressByEstimate(int totalEstimatedMinutes, int completedEstimatedMinutes)
    {
        if (totalEstimatedMinutes == 0)
            return 0;

        return Math.Round((decimal)completedEstimatedMinutes / totalEstimatedMinutes * 100, 2);
    }
}
