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
            var response = exception switch
            {
                ArgumentException argEx => StandardResponseObject<object>.BadRequest(argEx.Message, "Validation failed"),
                InvalidOperationException invOpEx => StandardResponseObject<object>.BadRequest(invOpEx.Message, "Operation conflict"),
                UnauthorizedAccessException unauthEx => StandardResponseObject<object>.BadRequest(unauthEx.Message, "Unauthorized access"),
                KeyNotFoundException keyNotFoundEx => StandardResponseObject<object>.NotFound(keyNotFoundEx.Message, "Resource not found"),
                SqlException sqlEx => StandardResponseObject<object>.InternalError("A database connection error occurred", "Database connection error"),
                TaskCanceledException tcEx when tcEx.InnerException is TimeoutException => StandardResponseObject<object>.InternalError("Request timed out", "Request timeout"),
                OperationCanceledException => StandardResponseObject<object>.InternalError("Request was cancelled", "Request cancelled"),
                _ => StandardResponseObject<object>.InternalError("An unexpected error occurred", "Internal server error")
            };

            var result = response.ToApiResult();
            await result.ExecuteAsync(context);
        }
    }
}
