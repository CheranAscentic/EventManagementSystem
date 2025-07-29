namespace EventManagementSystem.Application.Behavior
{
    using EventManagementSystem.Application.DTO;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> logger;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = this.validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var errorDict = failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(f => f.ErrorMessage).ToList()
                    );

                this.logger.LogWarning("Validation failed for {RequestType}. Errors: {Errors}", typeof(TRequest).Name, string.Join("; ", failures.Select(f => f.ErrorMessage)));
                throw new Common.Exceptions.ValidationsFailureException(errorDict);
            }

            return await next();
        }
    }
}
