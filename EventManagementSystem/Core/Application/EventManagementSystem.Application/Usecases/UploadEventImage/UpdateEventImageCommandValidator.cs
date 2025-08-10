namespace EventManagementSystem.Application.Usecases.UploadEventImage
{
    using FluentValidation;

    public class UpdateEventImageCommandValidator : AbstractValidator<UpdateEventImageCommand>
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 5MB

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public UpdateEventImageCommandValidator()
        {
            this.RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            this.RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => file != null && file.Length > 0).WithMessage("Image file cannot be empty.")
                .Must(file => file == null || file.Length <= MaxFileSizeBytes).WithMessage($"Image file size cannot exceed {MaxFileSizeBytes / (1024 * 1024)}MB.")
                .Must(file => file == null || IsValidImageFile(file.FileName)).WithMessage($"Image file must have a valid extension: {string.Join(", ", AllowedExtensions)}.");
        }

        private static bool IsValidImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }
}
