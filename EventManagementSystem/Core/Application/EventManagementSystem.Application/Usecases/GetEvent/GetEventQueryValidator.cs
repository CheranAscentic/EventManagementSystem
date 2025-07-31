namespace EventManagementSystem.Application.Usecases.GetEvent
{
    using FluentValidation;

    public class GetEventQueryValidator : AbstractValidator<GetEventQuery>
    {
        public GetEventQueryValidator()
        {
            this.RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId must not be empty.");
        }
    }
}
