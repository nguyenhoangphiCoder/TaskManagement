using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Features.Users.Queries.Login;

public record LoginQuery : IRequest<Result<AuthResultDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
