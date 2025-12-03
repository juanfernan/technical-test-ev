using FluentValidation;
using TestEv.Application.DTOs;
using TestEv.Domain.Entities;

namespace TestEv.Application.Validators
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.")
                .MaximumLength(100).WithMessage("OwnerId must not exceed 100 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(BeAValidStatus).WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<ProjectStatus>())}");
        }

        private static bool BeAValidStatus(string status)
        {
            return Enum.TryParse<ProjectStatus>(status, true, out _);
        }
    }

    public class UpdateProjectValidator : AbstractValidator<UpdateProjectRequest>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.")
                .MaximumLength(100).WithMessage("OwnerId must not exceed 100 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(BeAValidStatus).WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<ProjectStatus>())}");
        }

        private static bool BeAValidStatus(string status)
        {
            return Enum.TryParse<ProjectStatus>(status, true, out _);
        }
    }
}
