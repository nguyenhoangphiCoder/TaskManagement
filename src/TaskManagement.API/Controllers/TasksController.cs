using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Features.Tasks.Queries.ListTasks;
using TaskManagement.Application.Features.Tasks.Commands.AddComment;
using TaskManagement.Application.Features.Tasks.Commands.AddChecklist;
using TaskManagement.Application.Features.Tasks.Commands.ToggleChecklist;
using TaskManagement.Application.Features.Tasks.Commands.AddTagToTask;
using TaskManagement.Application.Features.Tasks.Commands.AssignTask;
using TaskManagement.Application.Features.Tasks.Commands.UnassignTask;
using TaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TaskManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TasksController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public TasksController(IMediator mediator, ILogger<TasksController> logger, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get paginated list of tasks with filtering and sorting
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks([FromQuery] ListTasksQuery query)
    {
        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get task by ID with full details
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailDto>> GetTaskById(Guid id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto taskDto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new CreateTaskCommand
        {
            Task = taskDto,
            TenantId = _currentUserService.TenantId.Value,
            CreatedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetTaskById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Get tasks by project
    /// </summary>
    [HttpGet("by-project/{projectId:guid}")]
    [ProducesResponseType(typeof(PaginatedList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasksByProject(Guid projectId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new ListTasksQuery { ProjectId = projectId, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get overdue tasks
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(PaginatedList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TaskDto>>> GetOverdueTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new ListTasksQuery { IsOverdue = true, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto taskDto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new UpdateTaskCommand
        {
            TaskId = id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            Priority = taskDto.Priority,
            StartDate = taskDto.StartDate,
            DueDate = taskDto.DueDate,
            EstimatedMinutes = taskDto.EstimatedMinutes,
            TenantId = _currentUserService.TenantId.Value,
            UpdatedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new DeleteTaskCommand
        {
            TaskId = id,
            TenantId = _currentUserService.TenantId.Value,
            DeletedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    /// <summary>
    /// Change task status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> ChangeTaskStatus(Guid id, [FromBody] Guid newStatusId)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new ChangeTaskStatusCommand
        {
            TaskId = id,
            NewStatusId = newStatusId,
            TenantId = _currentUserService.TenantId.Value,
            ChangedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Add a comment to a task
    /// </summary>
    [HttpPost("{id:guid}/comments")]
    [ProducesResponseType(typeof(TaskCommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskCommentDto>> AddComment(Guid id, [FromBody] CreateTaskCommentDto dto)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new AddCommentCommand
        {
            TaskId = id,
            Content = dto.Content,
            ParentCommentId = dto.ParentCommentId,
            TenantId = _currentUserService.TenantId.Value,
            AuthorId = _currentUserService.UserId.Value
        };
        
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get comments for a task
    /// </summary>
    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(List<TaskCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TaskCommentDto>>> GetTaskComments(Guid id)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var query = new TaskManagement.Application.Features.Tasks.Queries.GetTaskComments.GetTaskCommentsQuery
        {
            TaskId = id
        };

        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a comment from a task
    /// </summary>
    [HttpDelete("{id:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteComment(Guid id, Guid commentId)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new TaskManagement.Application.Features.Tasks.Commands.DeleteComment.DeleteCommentCommand
        {
            TaskId = id,
            CommentId = commentId,
            TenantId = _currentUserService.TenantId.Value,
            DeletedBy = _currentUserService.UserId.Value
        };

        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    /// <summary>
    /// Add a checklist to a task
    /// </summary>
    [HttpPost("{id:guid}/checklists")]
    [ProducesResponseType(typeof(TaskChecklistDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskChecklistDto>> AddChecklist(Guid id, [FromBody] CreateTaskChecklistDto dto)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var command = new AddChecklistCommand
        {
            TaskId = id,
            Checklist = dto,
            TenantId = _currentUserService.TenantId.Value
        };
        
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Toggle a checklist item completion status
    /// </summary>
    [HttpPatch("{id:guid}/checklists/{checklistId:guid}/toggle")]
    [ProducesResponseType(typeof(TaskChecklistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskChecklistDto>> ToggleChecklist(Guid id, Guid checklistId)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var command = new ToggleChecklistCommand
        {
            TaskId = id,
            ChecklistItemId = checklistId,
            TenantId = _currentUserService.TenantId.Value,
            ModifiedBy = _currentUserService.UserId.Value
        };
        
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Add a tag to a task
    /// </summary>
    [HttpPost("{id:guid}/tags/{tagId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddTagToTask(Guid id, Guid tagId)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var command = new AddTagToTaskCommand
        {
            TaskId = id,
            TagId = tagId,
            TenantId = _currentUserService.TenantId.Value
        };
        
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    /// <summary>
    /// Assign a member to a task
    /// </summary>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> AssignTask(Guid id, [FromQuery] Guid userId)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new AssignTaskCommand
        {
            TaskId = id,
            UserId = userId,
            TenantId = _currentUserService.TenantId.Value,
            AssignedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Remove a member from a task
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> UnassignTask(Guid id, Guid userId)
    {
        if (_currentUserService.TenantId == null || _currentUserService.UserId == null) return Unauthorized();

        var command = new UnassignTaskCommand
        {
            TaskId = id,
            UserId = userId,
            TenantId = _currentUserService.TenantId.Value,
            UnassignedBy = _currentUserService.UserId.Value
        };
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
