namespace EventManagementSystem.Application.Usecases.EventCreation
{
    using FluentValidation;
    using System;

    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            this.RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");
            this.RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
            this.RuleFor(x => x.EventDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");
            this.RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
            this.RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");
            this.RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
            this.RuleFor(x => x.RegistrationCutoffDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Registration cutoff date must be in the future.")
                .LessThan(x => x.EventDate).WithMessage("Registration cutoff date must be before the event date.");
        }
    }
}
