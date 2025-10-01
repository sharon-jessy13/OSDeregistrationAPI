using FluentValidation;
using OSDeregistrationAPI.Models;

namespace OSDeregistrationAPI.Validators;

public class DeregistrationRequestValidator : AbstractValidator<DeregistrationRequest>
{
    public DeregistrationRequestValidator()
    {
        RuleFor(x => x.OSMEmpID)
            .NotEmpty().WithMessage("Employee ID (OSMEmpID) is required.")
            .GreaterThan(0).WithMessage("A valid Employee ID must be provided.");

        RuleFor(x => x.ReasonID)
            .NotEmpty().WithMessage("ReasonID is required.")
            .GreaterThan(0).WithMessage("A valid ReasonID must be provided.");

        RuleFor(x => x.SkillsWorked)
            .MaximumLength(250).WithMessage("Skills description cannot exceed 250 characters.");
    }
}