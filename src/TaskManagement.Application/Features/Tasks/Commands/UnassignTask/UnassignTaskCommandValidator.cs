using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.UnassignTask;

public class UnassignTaskCommandValidator : AbstractValidator<UnassignTaskCommand>
{
    public UnassignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.UnassignedBy)
            .NotEmpty().WithMessage("Unassigned by user ID is required");
    }
}
