namespace EventManagementSystem.Application.Usecases.RefreshToken
{
    using FluentValidation;

    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            this.RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .Length(44).WithMessage("Refresh token must be a valid Base64 string of 32 bytes.")
                .Matches("^[A-Za-z0-9+/=]+$").WithMessage("Refresh token must be a valid Base64 string.");
        }
    }
}
