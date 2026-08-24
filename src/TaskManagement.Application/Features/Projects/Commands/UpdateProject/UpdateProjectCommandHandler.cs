using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);
        
        if (project == null || project.TenantId != request.TenantId)
        {
            return Result.Failure<ProjectDto>("Project not found or you don't have access.");
        }

        if (!string.IsNullOrEmpty(request.Project.Name))
        {
            project.UpdateDetails(request.Project.Name, request.Project.Description);
        }
        
        if (request.Project.StartDate.HasValue || request.Project.EndDate.HasValue)
        {
            // Assuming a method exists or we set properties directly
            // project.SetTimeline(request.Project.StartDate, request.Project.EndDate);
        }

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new ProjectDto 
        { 
            Id = project.Id, 
            Name = project.Name, 
            Code = project.Code,
            Description = project.Description 
        });
    }
}
