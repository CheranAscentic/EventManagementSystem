namespace EventManagementSystem.Application.Usecases.MakeUserEventRegistration
{
    using FluentValidation;

    public class MakeUserEventRegistrationCommandValidator : AbstractValidator<MakeUserEventRegistrationCommand>
    {
        public MakeUserEventRegistrationCommandValidator()
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId is required.");

            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number format is invalid. It should be 9 to 15 digits, optionally starting with '+'.");
        }
    }
}
