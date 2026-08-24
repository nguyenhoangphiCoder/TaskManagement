using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandValidator : AbstractValidator<ChangeTaskStatusCommand>
{
    public ChangeTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.NewStatusId)
            .NotEmpty().WithMessage("New status ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.ChangedBy)
            .NotEmpty().WithMessage("Changed by user ID is required");
    }
}
