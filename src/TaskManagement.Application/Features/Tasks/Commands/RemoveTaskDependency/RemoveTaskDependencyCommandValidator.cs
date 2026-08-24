using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.RemoveTaskDependency;

public class RemoveTaskDependencyCommandValidator : AbstractValidator<RemoveTaskDependencyCommand>
{
    public RemoveTaskDependencyCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.DependsOnTaskId)
            .NotEmpty().WithMessage("Depends on task ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Deleted by user ID is required");
    }
}
