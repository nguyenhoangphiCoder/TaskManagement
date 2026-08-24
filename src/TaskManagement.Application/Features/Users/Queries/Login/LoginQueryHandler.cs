using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Interfaces;
using System.Linq;

namespace TaskManagement.Application.Features.Users.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginQueryHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null || 
            string.IsNullOrEmpty(user.PasswordHash) || 
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResultDto>("Invalid email or password.");
        }

        user.RecordLogin();
        _unitOfWork.Users.Update(user);
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
