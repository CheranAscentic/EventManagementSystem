namespace EventManagementSystem.Application.Usecases.CancelUserEventRegistration
{
    using FluentValidation;

    public class CancelUserEventRegistrationCommandValidator : AbstractValidator<CancelUserEventRegistrationCommand>
    {
        public CancelUserEventRegistrationCommandValidator()
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId is required.");

            RuleFor(x => x.EventRegistrationId)
                .NotEmpty().WithMessage("EventRegistrationId is required.");
        }
    }
}
