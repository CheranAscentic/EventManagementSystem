namespace EventManagementSystem.Application.Usecases.GetEventsExtended
{
    using EventManagementSystem.Domain.Enums;
    using FluentValidation;

    /// <summary>
    /// Validator for GetEventsExtendedQuery using FluentValidation.
    /// Follows Single Responsibility Principle by handling only validation logic.
    /// Ensures data integrity before processing the query.
    /// </summary>
    public class GetEventsExtendedQueryValidator : AbstractValidator<GetEventsExtendedQuery>
    {
        public GetEventsExtendedQueryValidator()
        {
            this.RuleFor(x => x.ItemsPerPage)
                .GreaterThan(0).WithMessage("ItemsPerPage must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("ItemsPerPage cannot exceed 100 to prevent performance issues.");

            this.RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

            this.RuleFor(x => x.EventType)
                .Must(type => string.IsNullOrEmpty(type) || EventType.Types.Contains(type!))
                .WithMessage("EventType must be a valid event type.");

            this.RuleFor(x => x.StartDate)
                .Must((query, startDate) => !startDate.HasValue || !query.EndDate.HasValue || startDate <= query.EndDate)
                .WithMessage("StartDate must be before or equal to EndDate.");

            this.RuleFor(x => x.EndDate)
                .Must((query, endDate) => !endDate.HasValue || !query.StartDate.HasValue || endDate >= query.StartDate)
                .WithMessage("EndDate must be after or equal to StartDate.");

            this.When(x => !string.IsNullOrWhiteSpace(x.SearchTerm), () =>
            {
                this.RuleFor(x => x.SearchTerm)
                    .MinimumLength(2).WithMessage("SearchTerm must be at least 2 characters long.")
                    .MaximumLength(100).WithMessage("SearchTerm cannot exceed 100 characters.");
            });
        }
    }
}
