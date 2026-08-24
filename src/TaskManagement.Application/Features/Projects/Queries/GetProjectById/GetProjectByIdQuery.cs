using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid ProjectId) : IRequest<Result<ProjectDto>>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);
        
        if (project == null)
        {
            return Result.Failure<ProjectDto>("Project not found.");
        }

        return Result.Success(new ProjectDto 
        { 
            Id = project.Id, 
            Name = project.Name, 
            Code = project.Code,
            Description = project.Description 
        });
    }
}
