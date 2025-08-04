namespace EventManagementSystem.API.Services
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

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

            logger.LogDebug("MediatorPipelineService: Successfully executed mediator pipeline for request of type {RequestType}", request.GetType().Name);

            // Use dynamic to call ToApiResult on the result
            return ((dynamic)result).ToApiResult();
        }
    }
}
