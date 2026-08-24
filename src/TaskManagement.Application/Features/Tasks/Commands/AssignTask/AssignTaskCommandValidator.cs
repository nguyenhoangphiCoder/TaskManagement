using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.MemberType)
            .IsInEnum().WithMessage("Invalid member type");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.AssignedBy)
            .NotEmpty().WithMessage("Assigned by user ID is required");
    }
}
