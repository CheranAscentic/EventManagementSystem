namespace EventManagementSystem.Application.Usecases.UserRegistration
{
    using FluentValidation;

    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator()
        {
            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            this.RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(12).WithMessage("Passwords must be at least 12 characters.");

            this.RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            this.RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            this.RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required.")
                .MinimumLength(3).WithMessage("User name must be at least 3 characters long.");

            this.RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number format is invalid. It should be 9 to 15 digits, optionally starting with '+'.");
        }
    }
}
