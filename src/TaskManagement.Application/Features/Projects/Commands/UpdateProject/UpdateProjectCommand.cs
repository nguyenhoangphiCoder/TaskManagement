using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand : IRequest<Result<ProjectDto>>
{
    public Guid ProjectId { get; init; }
    public UpdateProjectDto Project { get; init; } = null!;
    public Guid TenantId { get; init; }
    public Guid UpdatedBy { get; init; }
}
