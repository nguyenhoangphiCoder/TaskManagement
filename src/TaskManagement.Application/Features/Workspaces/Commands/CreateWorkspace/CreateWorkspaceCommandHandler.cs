using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkspaceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceDto>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        // Validate tenant exists
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
            return Result.Failure<WorkspaceDto>("Tenant not found");

        // Validate owner exists
        var owner = await _unitOfWork.Users.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner == null)
            return Result.Failure<WorkspaceDto>("Owner user not found");

        // Generate slug
        var slug = WorkspaceSlug.Create(request.Name);

        // Check slug uniqueness
        var isUnique = await _unitOfWork.Workspaces.IsSlugUniqueAsync(slug, request.TenantId, cancellationToken);
        if (!isUnique)
            return Result.Failure<WorkspaceDto>("A workspace with this name already exists");

        // Create workspace
        var workspace = Workspace.Create(
            request.TenantId,
            request.Name,
            request.OwnerId,
            request.IsPrivate);

        if (!string.IsNullOrEmpty(request.Description))
            workspace.UpdateDetails(request.Name, request.Description);

        await _unitOfWork.Workspaces.AddAsync(workspace, cancellationToken);
        
        // Add default Task Statuses
        var statusTodo = Domain.Entities.TaskStatus.Create(request.TenantId, workspace.Id, "To Do", Domain.Enums.TaskStatusType.NotStarted, 0, "#94A3B8");
        var statusInProgress = Domain.Entities.TaskStatus.Create(request.TenantId, workspace.Id, "In Progress", Domain.Enums.TaskStatusType.InProgress, 1, "#3B82F6");
        var statusDone = Domain.Entities.TaskStatus.Create(request.TenantId, workspace.Id, "Done", Domain.Enums.TaskStatusType.Completed, 2, "#10B981");
        statusTodo.SetAsDefault();
        await _unitOfWork.TaskStatuses.AddRangeAsync(new[] { statusTodo, statusInProgress, statusDone }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var workspaceDto = workspace.Adapt<WorkspaceDto>();
        workspaceDto = workspaceDto with
        {
            Slug = workspace.Slug.Value,
            OwnerName = owner.FullName,
            MemberCount = workspace.Members.Count,
            ProjectCount = 0
        };

        return Result.Success(workspaceDto);
    }
}
