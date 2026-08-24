using FluentValidation;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by user ID is required");

        RuleFor(x => x.Project.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Project.Code)
            .NotEmpty().WithMessage("Project code is required")
            .MaximumLength(20).WithMessage("Project code cannot exceed 20 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Project code can only contain uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Project.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        When(x => x.Project.StartDate.HasValue && x.Project.EndDate.HasValue, () =>
        {
            RuleFor(x => x.Project)
                .Must(p => p.EndDate >= p.StartDate)
                .WithMessage("End date must be greater than or equal to start date");
        });
    }
}
