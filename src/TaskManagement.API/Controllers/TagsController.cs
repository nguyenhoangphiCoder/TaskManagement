using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Tags;
using TaskManagement.Application.Features.Tags.Commands.CreateTag;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TagsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Create a new tag for a workspace
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto dto)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var command = new CreateTagCommand
        {
            Tag = dto,
            TenantId = _currentUserService.TenantId.Value
        };
        
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        
        return CreatedAtAction(nameof(CreateTag), new { id = result.Value.Id }, result.Value);
    }
}
