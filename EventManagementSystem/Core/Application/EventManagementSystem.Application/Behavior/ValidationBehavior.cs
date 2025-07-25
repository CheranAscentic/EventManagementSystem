namespace EventManagementSystem.Application.Behavior
{
    using EventManagementSystem.Application.DTO;
    using FluentValidation;
    using MediatR;
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (this.validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    // Option 1: Throw an exception
                    // throw new ValidationException(failures);

                    // Option 2: Return a failed Result/StandardResponseObject (if using this pattern)
                    // You'll need to adapt this part depending on your response object
                    var responseType = typeof(TResponse);
                    if (responseType.IsGenericType &&
                        responseType.GetGenericTypeDefinition() == typeof(StandardResponseObject<>))
                    {
                        var errorMessages = string.Join("; ", failures.Select(f => f.ErrorMessage));
                        var response = Activator.CreateInstance(responseType, false, errorMessages, 400, null, errorMessages);
                        return (TResponse)response!;
                    }

                    throw new ValidationException(failures); // fallback
                }
            }

            return await next();
        }
    }
}
