using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.AddTaskDependency;

public class AddTaskDependencyCommandValidator : AbstractValidator<AddTaskDependencyCommand>
{
    public AddTaskDependencyCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.DependsOnTaskId)
            .NotEmpty().WithMessage("Depends on task ID is required");

        RuleFor(x => x)
            .Must(x => x.TaskId != x.DependsOnTaskId)
            .WithMessage("A task cannot depend on itself");

        RuleFor(x => x.DependencyType)
            .IsInEnum().WithMessage("Invalid dependency type");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by user ID is required");
    }
}
