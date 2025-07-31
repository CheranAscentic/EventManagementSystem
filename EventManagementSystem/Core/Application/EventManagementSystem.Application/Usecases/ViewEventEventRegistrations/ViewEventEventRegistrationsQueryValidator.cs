namespace EventManagementSystem.Application.Usecases.ViewEventEventRegistrations
{
    using FluentValidation;

    public class ViewEventEventRegistrationsQueryValidator : AbstractValidator<ViewEventEventRegistrationsQuery>
    {
        public ViewEventEventRegistrationsQueryValidator()
        {
            this.RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");
        }
    }
}
