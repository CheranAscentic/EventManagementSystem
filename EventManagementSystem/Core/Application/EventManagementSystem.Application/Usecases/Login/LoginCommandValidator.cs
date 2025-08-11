namespace EventManagementSystem.Application.Usecases.Login
{
    using FluentValidation;

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            this.RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}