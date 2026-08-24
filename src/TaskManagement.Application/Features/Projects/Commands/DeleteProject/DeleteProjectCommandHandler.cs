using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);
        
        if (project == null || project.TenantId != request.TenantId)
        {
            return Result.Failure<bool>("Project not found or access denied.");
        }

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(true);
    }
}
