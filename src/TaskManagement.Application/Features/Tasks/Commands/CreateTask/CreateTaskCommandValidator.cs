using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Task.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Task.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.Task.Description)
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Task.Description));

        RuleFor(x => x.Task.StatusId)
            .NotEmpty().WithMessage("StatusId is required");

        RuleFor(x => x.Task.DueDate)
            .GreaterThanOrEqualTo(x => x.Task.StartDate)
            .When(x => x.Task.StartDate.HasValue && x.Task.DueDate.HasValue)
            .WithMessage("Due date must be after start date");

        RuleFor(x => x.Task.EstimatedMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Estimated minutes cannot be negative");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required");
    }
}
