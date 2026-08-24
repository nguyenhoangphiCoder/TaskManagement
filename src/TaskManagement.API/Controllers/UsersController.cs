using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Features.Users.Queries.ListTenantUsers;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all users in the tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? search = null)
    {
        if (_currentUserService.TenantId == null) return Unauthorized();

        var result = await _mediator.Send(new ListTenantUsersQuery(_currentUserService.TenantId.Value, search));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    public ActionResult GetCurrentUser()
    {
        return Ok(new { 
            Id = _currentUserService.UserId, 
            TenantId = _currentUserService.TenantId
        });
    }
}
