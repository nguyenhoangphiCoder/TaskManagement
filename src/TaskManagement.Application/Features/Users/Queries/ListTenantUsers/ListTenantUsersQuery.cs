using MediatR;
using System;
using System.Collections.Generic;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Users;

namespace TaskManagement.Application.Features.Users.Queries.ListTenantUsers;

public record ListTenantUsersQuery(Guid TenantId, string? Search = null) : IRequest<Result<IEnumerable<UserDto>>>;
