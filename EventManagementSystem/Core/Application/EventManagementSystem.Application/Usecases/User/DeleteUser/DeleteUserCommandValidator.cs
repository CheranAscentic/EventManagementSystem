using FluentValidation;

namespace EventManagementSystem.Application.Usecases.User.DeleteUser
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
