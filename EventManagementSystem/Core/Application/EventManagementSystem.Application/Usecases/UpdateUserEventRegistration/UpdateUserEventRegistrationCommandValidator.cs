namespace EventManagementSystem.Application.Usecases.UpdateUserEventRegistration
{
    using FluentValidation;

    public class UpdateUserEventRegistrationCommandValidator : AbstractValidator<UpdateUserEventRegistrationCommand>
    {
        public UpdateUserEventRegistrationCommandValidator()
        {
            this.RuleFor(x => x.EventRegistrationId)
                .NotEmpty().WithMessage("EventRegistrationId is required.");

            this.RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            this.RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number format is invalid. It should be 9 to 15 digits, optionally starting with '+'.");
        }
    }
}