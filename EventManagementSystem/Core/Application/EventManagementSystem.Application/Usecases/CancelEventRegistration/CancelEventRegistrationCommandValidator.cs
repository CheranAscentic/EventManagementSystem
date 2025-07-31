namespace EventManagementSystem.Application.Usecases.CancelEventRegistration
{
    using FluentValidation;

    public class CancelEventRegistrationCommandValidator : AbstractValidator<CancelEventRegistrationCommand>
    {
        public CancelEventRegistrationCommandValidator()
        {
            this.RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId is required.");

            this.RuleFor(x => x.EventRegistrationId)
                .NotEmpty().WithMessage("EventRegistrationId is required.");
        }
    }
}
