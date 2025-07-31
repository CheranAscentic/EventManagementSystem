namespace EventManagementSystem.Application.Usecases.UploadEventImage
{
    using FluentValidation;

    public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            this.RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            this.RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var _)).WithMessage("ImageUrl must be a valid URL.");
        }
    }
}
