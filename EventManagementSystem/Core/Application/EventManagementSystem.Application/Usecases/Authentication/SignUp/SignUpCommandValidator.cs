namespace EventManagementSystem.Application.Usecases.Authentication.SignUp
{
    using FluentValidation;
    public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpCommandValidator()
        {
            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            this.RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            this.RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            this.RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");
        }
    }
}
