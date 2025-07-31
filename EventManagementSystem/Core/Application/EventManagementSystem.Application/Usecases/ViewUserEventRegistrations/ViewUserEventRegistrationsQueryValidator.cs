namespace EventManagementSystem.Application.Usecases.ViewUserEventRegistrations
{
    using FluentValidation;

    public class ViewUserEventRegistrationsQueryValidator : AbstractValidator<ViewUserEventRegistrationsQuery>
    {
        public ViewUserEventRegistrationsQueryValidator()
        {
            this.RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId is required.");
        }
    }
}
