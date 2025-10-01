using FluentValidation;
using OSDeregistrationAPI.Models;

namespace OSDeregistrationAPI.Validators;

public class ClearanceSubmissionValidator : AbstractValidator<ClearanceSubmission>
{
    public ClearanceSubmissionValidator()
    {
        RuleFor(x => x.MasterId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.ActedByMempId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one clearance item must be submitted.");
    }
}