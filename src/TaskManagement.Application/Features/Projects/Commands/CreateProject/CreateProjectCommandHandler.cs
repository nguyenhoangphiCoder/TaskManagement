using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Validate workspace exists
        var workspace = await _unitOfWork.Workspaces.GetByIdAsync(request.WorkspaceId, cancellationToken);
        if (workspace == null)
            return Result.Failure<ProjectDto>("Workspace not found");

        // Validate workspace belongs to tenant
        if (workspace.TenantId != request.TenantId)
            return Result.Failure<ProjectDto>("Access denied");

        // Check if project code is unique within workspace
        var isCodeUnique = await _unitOfWork.Projects.IsCodeUniqueAsync(
            request.Project.Code,
            request.WorkspaceId,
            cancellationToken);

        if (!isCodeUnique)
            return Result.Failure<ProjectDto>($"Project code '{request.Project.Code}' already exists in this workspace");

        // Create project using the correct factory method
        var project = Project.Create(
            request.TenantId,
            request.WorkspaceId,
            request.Project.Name,
            request.Project.Code,
            request.Project.IsPublicToWorkspace);

        // Update description if provided
        if (!string.IsNullOrEmpty(request.Project.Description))
        {
            project.UpdateDetails(request.Project.Name, request.Project.Description);
        }

        // Set timeline if dates provided
        if (request.Project.StartDate.HasValue && request.Project.EndDate.HasValue)
        {
            project.SetTimeline(request.Project.StartDate.Value, request.Project.EndDate.Value);
        }

        // Add creator as project owner
        project.AddMember(request.CreatedBy, MemberRole.Owner);

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var projectDto = project.Adapt<ProjectDto>();
        projectDto = projectDto with
        {
            WorkspaceName = workspace.Name,
            TaskCount = 0,
            MemberCount = 1
        };

        return Result.Success(projectDto);
    }
}
