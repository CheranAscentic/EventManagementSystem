namespace EventManagementSystem.Application.Usecases.AdminLogin
{
    using FluentValidation;

    public class AdminLoginCommandValidator : AbstractValidator<AdminLoginCommand>
    {
        public AdminLoginCommandValidator()
        {
            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            this.RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
