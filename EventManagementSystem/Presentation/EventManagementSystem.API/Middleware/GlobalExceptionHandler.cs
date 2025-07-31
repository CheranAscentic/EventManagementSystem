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

            switch (exception)
            {
                case EventManagementSystem.Application.Common.Exceptions.ValidationsFailureException validationEx:
                    status = 400;
                    error = "Validation failed";
                    message = validationEx.Message;
                    value = validationEx.Errors;
                    break;
                case ArgumentException argEx:
                    status = 400;
                    error = "Validation failed";
                    message = argEx.Message;
                    break;
                case InvalidOperationException invOpEx:
                    status = 409;
                    error = "Operation conflict";
                    message = invOpEx.Message;
                    break;
                case UnauthorizedAccessException unauthEx:
                    status = 401;
                    error = "Unauthorized access";
                    message = unauthEx.Message;
                    break;
                case KeyNotFoundException keyNotFoundEx:
                    status = 404;
                    error = "Resource not found";
                    message = keyNotFoundEx.Message;
                    break;
                case SqlException sqlEx:
                    status = 500;
                    error = "Database connection error";
                    message = "A database connection error occurred";
                    break;
                case TaskCanceledException tcEx when tcEx.InnerException is TimeoutException:
                    status = 500;
                    error = "Request timeout";
                    message = "Request timed out";
                    break;
                case OperationCanceledException:
                    status = 500;
                    error = "Request cancelled";
                    message = "Request was cancelled";
                    break;
                default:
                    status = 500;
                    error = "Internal server error";
                    message = "An unexpected error occurred";
                    break;
            }

            var response = Result<object>.Failure(message, value, status, error);
            var result = response.ToApiResult();
            await result.ExecuteAsync(context);
        }
    }
}
