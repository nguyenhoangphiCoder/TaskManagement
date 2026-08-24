using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskDetailDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken);

        if (task == null)
            return Result.Failure<TaskDetailDto>("Task not found");

        var taskDto = task.Adapt<TaskDetailDto>();
        taskDto = taskDto with
        {
            Code = task.Code.Value,
            StatusName = task.Status.Name,
            StatusColor = task.Status.Color,
            ProjectName = task.Project.Name,
            ProjectCode = task.Project.Code,
            IsOverdue = task.IsOverdue()
        };

        return Result.Success(taskDto);
    }
}
