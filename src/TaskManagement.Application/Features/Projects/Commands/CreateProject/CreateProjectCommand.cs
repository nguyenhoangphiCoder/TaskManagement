using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<Result<ProjectDto>>
{
    public CreateProjectDto Project { get; init; } = null!;
    public Guid TenantId { get; init; }
    public Guid WorkspaceId { get; init; }
    public Guid CreatedBy { get; init; }
}
