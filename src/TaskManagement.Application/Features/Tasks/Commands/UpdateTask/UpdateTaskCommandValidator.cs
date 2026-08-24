using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("Updated by user ID is required");

        When(x => !string.IsNullOrEmpty(x.Title), () =>
        {
            RuleFor(x => x.Title)
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");
        });

        When(x => x.EstimatedMinutes.HasValue, () =>
        {
            RuleFor(x => x.EstimatedMinutes!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated minutes cannot be negative");
        });

        When(x => x.StartDate.HasValue && x.DueDate.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.DueDate >= x.StartDate)
                .WithMessage("Due date must be greater than or equal to start date");
        });
    }
}
