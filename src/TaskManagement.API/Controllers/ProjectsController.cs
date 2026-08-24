using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Application.Features.Projects.Queries.ListProjects;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ProjectsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto projectDto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new CreateProjectCommand
        {
            Project = projectDto,
            TenantId = _currentUserService.TenantId.Value,
            WorkspaceId = projectDto.WorkspaceId,
            CreatedBy = _currentUserService.UserId.Value
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created(string.Empty, result.Value);
    }
    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectDto projectDto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new UpdateProjectCommand
        {
            ProjectId = id,
            Project = projectDto,
            TenantId = _currentUserService.TenantId.Value,
            UpdatedBy = _currentUserService.UserId.Value
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new DeleteProjectCommand
        {
            ProjectId = id,
            TenantId = _currentUserService.TenantId.Value,
            DeletedBy = _currentUserService.UserId.Value
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetProjectById(Guid id)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// List projects in a workspace
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedList<ProjectDto>>> ListProjects([FromQuery] Guid workspaceId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        if (workspaceId == Guid.Empty)
            return BadRequest(new { error = "WorkspaceId is required." });

        var query = new ListProjectsQuery
        {
            WorkspaceId = workspaceId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}
