using EventManagementSystem.Application.DTO;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace EventManagementSystem.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionHandler> logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("GlobalExceptionHandler caught an exception for request {RequestMethod} {RequestPath}", context.Request.Method, context.Request.Path);
                this.logger.LogDebug("Exception type: {ExceptionType}, Message: {ExceptionMessage}", ex.GetType().FullName, ex.Message);
                this.logger.LogDebug("Stack Trace: {StackTrace}", ex.StackTrace);
                this.logger.LogError(
                    ex,
                    "An unhandled exception occurred while processing request {RequestMethod} {RequestPath}",
                    context.Request.Method,
                    context.Request.Path
                );

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int status;
            string error;
            string message;
            object? value = null;

            this.logger.LogInformation("Handling exception of type {ExceptionType}", exception.GetType().FullName);
            this.logger.LogDebug("Exception message: {ExceptionMessage}", exception.Message);

            switch (exception)
            {
                case EventManagementSystem.Application.Common.Exceptions.ValidationsFailureException validationEx:
                    status = 400;
                    error = "Validation failed";
                    message = validationEx.Message;
                    value = validationEx.Errors;
                    this.logger.LogWarning("Validation failure: {ValidationMessage}", validationEx.Message);
                    break;
                case ArgumentException argEx:
                    status = 400;
                    error = "Validation failed";
                    message = argEx.Message;
                    this.logger.LogWarning("Argument exception: {ArgumentMessage}", argEx.Message);
                    break;
                case InvalidOperationException invOpEx:
                    status = 409;
                    error = "Operation conflict";
                    message = invOpEx.Message;
                    this.logger.LogWarning("Invalid operation: {OperationMessage}", invOpEx.Message);
                    break;
                case UnauthorizedAccessException unauthEx:
                    status = 401;
                    error = "Unauthorized access";
                    message = unauthEx.Message;
                    this.logger.LogWarning("Unauthorized access: {UnauthorizedMessage}", unauthEx.Message);
                    break;
                case KeyNotFoundException keyNotFoundEx:
                    status = 404;
                    error = "Resource not found";
                    message = keyNotFoundEx.Message;
                    this.logger.LogWarning("Resource not found: {ResourceMessage}", keyNotFoundEx.Message);
                    break;
                case SqlException sqlEx:
                    status = 500;
                    error = "Database connection error";
                    message = "A database connection error occurred";
                    this.logger.LogWarning("Database connection error: {SqlMessage}", sqlEx.Message);
                    break;
                case TaskCanceledException tcEx when tcEx.InnerException is TimeoutException:
                    status = 500;
                    error = "Request timeout";
                    message = "Request timed out";
                    this.logger.LogWarning("Request timeout: {TimeoutMessage}", tcEx.Message);
                    break;
                case OperationCanceledException:
                    status = 500;
                    error = "Request cancelled";
                    message = "Request was cancelled";
                    this.logger.LogWarning("Request cancelled: {CancelMessage}", exception.Message);
                    break;
                case JsonException jsonEx:
                    status = 500;
                    error = "Serialization error";
                    message = "A serialization error occurred. Possible object cycle detected.";
                    this.logger.LogWarning("Serialization error: {JsonMessage}", jsonEx.Message);
                    break;
                default:
                    status = 500;
                    error = "Internal server error";
                    message = "An unexpected error occurred";
                    this.logger.LogError(exception, "Unhandled exception: {ExceptionMessage}", exception.Message);
                    break;
            }

            var response = Result<object>.Failure(message, value, status, error);
            var result = response.ToApiResult();
            await result.ExecuteAsync(context);
        }
    }
}
