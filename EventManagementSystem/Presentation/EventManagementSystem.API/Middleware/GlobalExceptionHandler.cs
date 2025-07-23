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
                await next(context);
            }
            catch (Exception ex)
            {
                //using (LogContext.PushProperty("UserId", context.User?.Identity?.Name ?? "Anonymous"))
                //using (LogContext.PushProperty("RequestPath", context.Request.Path))
                //using (LogContext.PushProperty("RequestMethod", context.Request.Method))
                //{
                //    Log.Error(ex, "An unhandled exception occurred while processing request {RequestMethod} {RequestPath}",
                //        context.Request.Method, context.Request.Path);
                //}

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, response) = exception switch
            {
                ArgumentException argEx => (
                    (int)HttpStatusCode.BadRequest,
                    StandardResponseObject<object>.BadRequest(argEx.Message, "Validation failed")
                ),

                InvalidOperationException invOpEx => (
                    (int)HttpStatusCode.Conflict,
                    StandardResponseObject<object>.BadRequest(invOpEx.Message, "Operation conflict")
                ),

                UnauthorizedAccessException unauthEx => (
                    (int)HttpStatusCode.Unauthorized,
                    StandardResponseObject<object>.BadRequest(unauthEx.Message, "Unauthorized access")
                ),

                KeyNotFoundException keyNotFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    StandardResponseObject<object>.NotFound(keyNotFoundEx.Message, "Resource not found")
                ),

                //DbUpdateException dbEx => (
                //    (int)HttpStatusCode.InternalServerError,
                //    StandardResponseObject<object>.InternalError("A database error occurred", "Database error")
                //),

                SqlException sqlEx => (
                    (int)HttpStatusCode.InternalServerError,
                    StandardResponseObject<object>.InternalError("A database connection error occurred", "Database connection error")
                ),

                TaskCanceledException tcEx when tcEx.InnerException is TimeoutException => (
                    (int)HttpStatusCode.RequestTimeout,
                    StandardResponseObject<object>.InternalError("Request timed out", "Request timeout")
                ),

                OperationCanceledException => (
                    499, // Client Closed Request
                    StandardResponseObject<object>.InternalError("Request was cancelled", "Request cancelled")
                ),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    StandardResponseObject<object>.InternalError("An unexpected error occurred", "Internal server error")
                )
            };

            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
