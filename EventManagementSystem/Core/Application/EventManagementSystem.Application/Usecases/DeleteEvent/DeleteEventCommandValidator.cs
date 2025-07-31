namespace EventManagementSystem.Application.Usecases.DeleteEvent
{
    using FluentValidation;

    public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
    {
        public DeleteEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Event Id is required.")
                .Must(id => id != Guid.Empty).WithMessage("Event Id must be a valid GUID.");
        }
    }
}
