using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Users.Queries.ListTenantUsers;

public class ListTenantUsersQueryHandler : IRequestHandler<ListTenantUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListTenantUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(ListTenantUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetByTenantAsync(request.TenantId, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            users = users.Where(u => u.FullName.ToLowerInvariant().Contains(search) || u.Email.ToLowerInvariant().Contains(search));
        }

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            AvatarUrl = u.AvatarUrl,
            Status = u.Status.ToString()
        }).OrderBy(u => u.FullName).ToList();

        return Result.Success<IEnumerable<UserDto>>(dtos);
    }
}
