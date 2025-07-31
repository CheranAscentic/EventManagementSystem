namespace EventManagementSystem.Application.Usecases.CreateEvent
{
    using FluentValidation;
    using System;

    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.EventDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");
            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
            RuleFor(x => x.RegistrationCutoffDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Registration cutoff date must be in the future.")
                .LessThan(x => x.EventDate).WithMessage("Registration cutoff date must be before the event date.");
        }
    }
}
