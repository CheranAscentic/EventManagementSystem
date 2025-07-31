namespace EventManagementSystem.Application.Usecases.UpdateEvent
{
    using FluentValidation;
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            this.RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Event Id is required.");

            this.RuleFor(x => x.Title)
                .Must(t => t == null || !string.IsNullOrWhiteSpace(t))
                .WithMessage("Title, if provided, must not be empty.");

            this.RuleFor(x => x.Description)
                .Must(d => d == null || !string.IsNullOrWhiteSpace(d))
                .WithMessage("Description, if provided, must not be empty.");

            this.RuleFor(x => x.Location)
                .Must(l => l == null || !string.IsNullOrWhiteSpace(l))
                .WithMessage("Location, if provided, must not be empty.");

            this.RuleFor(x => x.Type)
                .Must(t => t == null || !string.IsNullOrWhiteSpace(t))
                .WithMessage("Type, if provided, must not be empty.");

            this.RuleFor(x => x.Capacity)
                .Must(c => c == null || c > 0)
                .WithMessage("Capacity, if provided, must be greater than zero.");

            this.RuleFor(x => x.EventDate)
                .Must(ed => ed == null || ed > DateTime.UtcNow)
                .WithMessage("Event date must be in the future.");

            this.RuleFor(x => x.RegistrationCutoffDate)
                .Must(rcd => rcd == null || rcd > DateTime.UtcNow)
                .WithMessage("Registration cutoff date must be in the future.")
                .Must((cmd, rcd) => rcd == null || cmd.EventDate == null || rcd < cmd.EventDate)
                .WithMessage("Registration cutoff date must be before the event date.");
        }
    }
}
