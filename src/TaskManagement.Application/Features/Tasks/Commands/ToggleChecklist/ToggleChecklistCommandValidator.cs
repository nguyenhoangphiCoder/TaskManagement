using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.ToggleChecklist;

public class ToggleChecklistCommandValidator : AbstractValidator<ToggleChecklistCommand>
{
    public ToggleChecklistCommandValidator()
    {
        RuleFor(x => x.ChecklistItemId)
            .NotEmpty().WithMessage("Checklist item ID is required");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.ModifiedBy)
            .NotEmpty().WithMessage("Modified by user ID is required");
    }
}
