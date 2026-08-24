using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Workspaces;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Workspaces.Commands.CreateWorkspace;
using TaskManagement.Application.Features.Workspaces.Commands.UpdateWorkspace;
using TaskManagement.Application.Features.Workspaces.Commands.ArchiveWorkspace;
using TaskManagement.Application.Features.Workspaces.Commands.InviteMember;
using TaskManagement.Application.Features.Workspaces.Commands.RemoveMember;
using TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceById;
using TaskManagement.Application.Features.Workspaces.Queries.ListUserWorkspaces;
using TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceStatuses;
using TaskManagement.Application.Features.Workspaces.Queries.GetWorkspaceMembers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public WorkspacesController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all workspaces the current user is a member of
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkspaceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkspaceDto>>> GetUserWorkspaces()
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var result = await _mediator.Send(new ListUserWorkspacesQuery(_currentUserService.TenantId.Value, _currentUserService.UserId.Value));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get workspace by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkspaceDto>> GetWorkspaceById(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var result = await _mediator.Send(new GetWorkspaceByIdQuery(id, _currentUserService.TenantId.Value, _currentUserService.UserId.Value));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get all task statuses in a workspace
    /// </summary>
    [HttpGet("{id:guid}/statuses")]
    [ProducesResponseType(typeof(IEnumerable<TaskStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TaskStatusDto>>> GetWorkspaceStatuses(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var result = await _mediator.Send(new GetWorkspaceStatusesQuery(id, _currentUserService.TenantId.Value, _currentUserService.UserId.Value));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get all members in a workspace
    /// </summary>
    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(typeof(IEnumerable<WorkspaceMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<WorkspaceMemberDto>>> GetWorkspaceMembers(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var result = await _mediator.Send(new GetWorkspaceMembersQuery(id, _currentUserService.TenantId.Value, _currentUserService.UserId.Value));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new workspace
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkspaceDto>> CreateWorkspace([FromBody] CreateWorkspaceDto dto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new CreateWorkspaceCommand
        {
            Name = dto.Name,
            Description = dto.Description,
            IsPrivate = dto.IsPrivate,
            TenantId = _currentUserService.TenantId.Value,
            OwnerId = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetWorkspaceById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Update an existing workspace
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkspaceDto>> UpdateWorkspace(Guid id, [FromBody] UpdateWorkspaceDto dto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new UpdateWorkspaceCommand
        {
            WorkspaceId = id,
            Workspace = dto,
            TenantId = _currentUserService.TenantId.Value,
            UpdatedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Archive a workspace
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ArchiveWorkspace(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new ArchiveWorkspaceCommand
        {
            WorkspaceId = id,
            TenantId = _currentUserService.TenantId.Value,
            ArchivedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    /// <summary>
    /// Invite a member to the workspace
    /// </summary>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> InviteMember(Guid id, [FromQuery] Guid userIdToInvite, [FromQuery] MemberRole role = MemberRole.Member)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new InviteMemberCommand
        {
            WorkspaceId = id,
            UserIdToInvite = userIdToInvite,
            Role = role,
            TenantId = _currentUserService.TenantId.Value,
            InvitedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    /// <summary>
    /// Remove a member from the workspace
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveMember(Guid id, Guid userId)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new RemoveMemberCommand
        {
            WorkspaceId = id,
            UserIdToRemove = userId,
            TenantId = _currentUserService.TenantId.Value,
            RemovedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }
}
