namespace EventManagementSystem.API.Services
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Interfaces;
    using MediatR;
    using System.Collections;

    public class MediatorPipelineService
    {
        public async Task<IResult> ExecuteAsync(
            object request,
            IMediator mediator,
            ILogger logger)
        {
            logger.LogDebug("MediatorPipelineService: Executing mediator pipeline for request of type {RequestType}", request.GetType().Name);
            var result = await mediator.Send(request);
            if (result == null)
            {
                logger.LogWarning("MediatorPipelineService: Mediator returned null for request of type {RequestType}", request.GetType().Name);
                var failureResult = Result<object>.Failure("No result", null, 500, "Null result");
                return failureResult.ToApiResult();
            }

            logger.LogDebug("MediatorPipelineService: Successfully executed mediator pipeline for request of type {RequestType}, and received result of type {ResultType}", request.GetType().Name, result.GetType().Name);

            // Log the type of Value property if result is Result<T>
            var resultType = result.GetType();
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var value = resultType.GetProperty("Value")?.GetValue(result);
                string valueType = "none";
                if (value is IEnumerable enumerable && !(value is string))
                {
                    foreach (var item in enumerable)
                    {
                        valueType = item.GetType().Name;
                        break;
                    }
                }
                else if (value is IsDto)
                {
                    valueType = "IsDto";
                }
                else if (value is HasDto)
                {
                    valueType = "HasDto";
                }
                else if (value != null)
                {
                    valueType = value.GetType().Name;
                }

                logger.LogDebug("MediatorPipelineService: Result.Value type is {ValueType}", valueType);
            }

            if (result is Result<object> isResult) 
            {
                return isResult.ToApiResult();
            }

            // Use dynamic to call ToApiResult on the result
            return ((dynamic)result).ToApiResult();
        }
    }
}
