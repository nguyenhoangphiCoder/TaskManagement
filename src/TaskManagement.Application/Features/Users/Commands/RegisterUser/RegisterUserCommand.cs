using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<Result<AuthResultDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}
