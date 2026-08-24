using Mapster;
using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tags;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tags.Commands.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<TagDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TagDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // First ensure Workspace exists
        var workspace = await _unitOfWork.Workspaces.GetByIdAsync(request.Tag.WorkspaceId, cancellationToken);
        if (workspace == null || workspace.TenantId != request.TenantId)
            return Result.Failure<TagDto>("Workspace not found");

        var tag = Tag.Create(request.TenantId, request.Tag.WorkspaceId, request.Tag.Name, request.Tag.Color);

        await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = tag.Adapt<TagDto>();
        return Result.Success(dto);
    }
}
