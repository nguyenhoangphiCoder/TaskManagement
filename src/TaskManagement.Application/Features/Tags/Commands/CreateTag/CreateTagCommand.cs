using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tags;
using System;

namespace TaskManagement.Application.Features.Tags.Commands.CreateTag;

public record CreateTagCommand : IRequest<Result<TagDto>>
{
    public CreateTagDto Tag { get; init; } = null!;
    public Guid TenantId { get; init; }
}
