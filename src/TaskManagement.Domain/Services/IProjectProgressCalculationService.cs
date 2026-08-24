namespace TaskManagement.Domain.Services;

public interface IProjectProgressCalculationService
{
    decimal CalculateProgress(int totalTasks, int completedTasks);
    decimal CalculateProgressByEstimate(int totalEstimatedMinutes, int completedEstimatedMinutes);
}
