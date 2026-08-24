using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using System.Linq;

namespace TaskManagement.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResultDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUsers.Any())
        {
            return Result.Failure<AuthResultDto>("Email is already registered.");
        }

        // For simplicity, we create a dummy tenant for the user for now.
        // In a real multi-tenant app, the user might be invited to a tenant.
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(tenantId, request.Email, request.FullName, passwordHash);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtProvider.GenerateToken(user);

        return Result.Success(new AuthResultDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token
        });
    }
}
