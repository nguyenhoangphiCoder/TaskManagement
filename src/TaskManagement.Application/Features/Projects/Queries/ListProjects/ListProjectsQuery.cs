using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.Interfaces;
using System.Linq;

namespace TaskManagement.Application.Features.Projects.Queries.ListProjects;

public record ListProjectsQuery : IRequest<Result<PaginatedList<ProjectDto>>>
{
    public Guid WorkspaceId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, Result<PaginatedList<ProjectDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<ProjectDto>>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        // Simple mock implementation for now since actual filtering/sorting is usually done via IQueryable in Repository
        var allProjects = await _unitOfWork.Projects.GetAllAsync();
        var workspaceProjects = allProjects.Where(p => p.WorkspaceId == request.WorkspaceId).ToList();

        var count = workspaceProjects.Count;
        var items = workspaceProjects.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(p => new ProjectDto 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Code = p.Code,
                Description = p.Description 
            }).ToList();

        return Result.Success(new PaginatedList<ProjectDto>(items, count, request.PageNumber, request.PageSize));
    }
}
