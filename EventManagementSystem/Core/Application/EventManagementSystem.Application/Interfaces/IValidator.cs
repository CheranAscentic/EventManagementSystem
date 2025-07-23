namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Application.Common;
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
    }
}
