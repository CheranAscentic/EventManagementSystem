namespace EventManagementSystem.API.Endpoints
{
    using System.Security.Claims;
    using System.Text.Json;
    using EventManagementSystem.API.Interface;
    using EventManagementSystem.API.Authorizations;
    using EventManagementSystem.Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Debug endpoint for JWT token inspection and testing
    /// </summary>
    public class DebugEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var debug = app.MapGroup("/api/debug")
                .WithTags("Debug Endpoints")
                .WithOpenApi();

            debug.MapGet("/token", HandleDebugToken)
                .WithName("DebugToken")
                .WithSummary("Debug and inspect the current JWT token details")
                .WithDescription("Returns detailed information about the current user's JWT token including all claims, roles, and authentication status")
                .Produces<object>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .RequireAuthorization(AuthorizationPolicies.DebugToken);

            debug.MapGet("/token/raw", HandleDebugTokenRaw)
                .WithName("DebugTokenRaw")
                .WithSummary("Get raw JWT token information without custom policy")
                .WithDescription("Returns basic token information for any authenticated user")
                .Produces<object>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

            debug.MapGet("/user", HandleDebugCurrentUser)
                .WithName("DebugCurrentUser")
                .WithSummary("Debug current user context from CurrentUserService")
                .WithDescription("Returns information from ICurrentUserService about the current user")
                .Produces<object>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

            // Add a completely open endpoint to debug request headers
            debug.MapGet("/headers", HandleDebugHeaders)
                .WithName("DebugHeaders")
                .WithSummary("Debug request headers and authentication information")
                .WithDescription("Returns all request headers and basic authentication info - no authorization required")
                .Produces<object>(StatusCodes.Status200OK)
                .AllowAnonymous();
        }

        private static Task<IResult> HandleDebugToken(
            HttpContext httpContext,
            [FromServices] ILogger<DebugEndpoint> logger)
        {
            logger.LogInformation("DebugToken request received");

            var tokenInfo = ExtractTokenInformation(httpContext, logger);
            
            logger.LogInformation("Token debug information extracted for user: {UserId}", tokenInfo.GetType().GetProperty("summary")?.GetValue(tokenInfo)?.GetType().GetProperty("userId")?.GetValue(tokenInfo.GetType().GetProperty("summary")?.GetValue(tokenInfo)));
            
            var result = Results.Ok(new
            {
                success = true,
                message = "JWT Token Debug Information",
                data = tokenInfo,
                timestamp = DateTime.UtcNow
            });

            return Task.FromResult(result);
        }

        private static Task<IResult> HandleDebugTokenRaw(
            HttpContext httpContext,
            [FromServices] ILogger<DebugEndpoint> logger)
        {
            logger.LogInformation("DebugTokenRaw request received");

            var user = httpContext.User;
            var rawInfo = new
            {
                isAuthenticated = user.Identity?.IsAuthenticated ?? false,
                authenticationType = user.Identity?.AuthenticationType,
                name = user.Identity?.Name,
                claimsCount = user.Claims.Count(),
                identitiesCount = user.Identities.Count()
            };

            logger.LogDebug("Raw token info: {@RawInfo}", rawInfo);
            
            var result = Results.Ok(new
            {
                success = true,
                message = "Raw JWT Token Information",
                data = rawInfo,
                timestamp = DateTime.UtcNow
            });

            return Task.FromResult(result);
        }

        private static Task<IResult> HandleDebugCurrentUser(
            [FromServices] ICurrentUserService currentUserService,
            [FromServices] ILogger<DebugEndpoint> logger)
        {
            logger.LogInformation("DebugCurrentUser request received");

            var currentUserInfo = new
            {
                userId = currentUserService.GetCurrentUserId(),
                email = currentUserService.GetCurrentUserEmail(),
                isAdmin = currentUserService.IsCurrentUserAdmin(),
                isAuthenticated = currentUserService.IsAuthenticated()
            };

            logger.LogInformation("Current user info: {@CurrentUserInfo}", currentUserInfo);
            
            var result = Results.Ok(new
            {
                success = true,
                message = "Current User Service Information",
                data = currentUserInfo,
                timestamp = DateTime.UtcNow
            });

            return Task.FromResult(result);
        }

        private static Task<IResult> HandleDebugHeaders(
            HttpContext httpContext,
            [FromServices] ILogger<DebugEndpoint> logger)
        {
            logger.LogInformation("DebugHeaders request received (no auth required)");

            var headers = httpContext.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
            var user = httpContext.User;

            var debugInfo = new
            {
                requestInfo = new
                {
                    scheme = httpContext.Request.Scheme,
                    host = httpContext.Request.Host.ToString(),
                    path = httpContext.Request.Path.ToString(),
                    method = httpContext.Request.Method
                },
                authorizationHeader = new
                {
                    present = !string.IsNullOrEmpty(authHeader),
                    value = authHeader ?? "null",
                    scheme = authHeader?.Split(' ').FirstOrDefault() ?? "null",
                    tokenLength = authHeader?.Split(' ').LastOrDefault()?.Length ?? 0,
                    tokenPrefix = authHeader?.Split(' ').LastOrDefault()?.Substring(0, Math.Min(20, authHeader?.Split(' ').LastOrDefault()?.Length ?? 0)) ?? "null"
                },
                allHeaders = headers,
                userContext = new
                {
                    isAuthenticated = user.Identity?.IsAuthenticated ?? false,
                    authenticationType = user.Identity?.AuthenticationType ?? "null",
                    name = user.Identity?.Name ?? "null",
                    claimsCount = user.Claims.Count(),
                    hasAnyClaims = user.Claims.Any()
                },
                middleware = new
                {
                    requestId = httpContext.TraceIdentifier,
                    connectionId = httpContext.Connection.Id,
                    remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "null"
                }
            };

            logger.LogInformation("Debug headers info: {@DebugInfo}", debugInfo);
            
            var result = Results.Ok(new
            {
                success = true,
                message = "Request Headers and Authentication Debug Information",
                data = debugInfo,
                timestamp = DateTime.UtcNow
            });

            return Task.FromResult(result);
        }

        private static object ExtractTokenInformation(HttpContext httpContext, ILogger logger)
        {
            var user = httpContext.User;

            // Extract all claims
            var claims = user.Claims.Select(c => new
            {
                type = c.Type,
                value = c.Value,
                valueType = c.ValueType,
                issuer = c.Issuer,
                originalIssuer = c.OriginalIssuer
            }).ToList();

            // Extract roles
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Extract key claims
            var keyClaimsInfo = new
            {
                nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                email = user.FindFirst(ClaimTypes.Email)?.Value,
                name = user.FindFirst(ClaimTypes.Name)?.Value,
                sub = user.FindFirst("sub")?.Value,
                jwtEmail = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                exp = user.FindFirst("exp")?.Value,
                iss = user.FindFirst("iss")?.Value,
                aud = user.FindFirst("aud")?.Value
            };

            // Identity information
            var identities = user.Identities.Select(i => new
            {
                name = i.Name,
                authenticationType = i.AuthenticationType,
                isAuthenticated = i.IsAuthenticated,
                claimsCount = i.Claims.Count()
            }).ToList();

            // Role checks
            var roleChecks = new
            {
                isAdmin = user.IsInRole("Admin"),
                isUser = user.IsInRole("User"),
                hasAnyRole = roles.Any()
            };

            // Authorization header info
            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
            var tokenInfo = new
            {
                hasAuthorizationHeader = !string.IsNullOrEmpty(authHeader),
                authorizationScheme = authHeader?.Split(' ').FirstOrDefault(),
                tokenLength = authHeader?.Split(' ').LastOrDefault()?.Length ?? 0,
                tokenPrefix = authHeader?.Split(' ').LastOrDefault()?.Substring(0, Math.Min(10, authHeader?.Split(' ').LastOrDefault()?.Length ?? 0))
            };

            var result = new
            {
                authentication = new
                {
                    isAuthenticated = user.Identity?.IsAuthenticated ?? false,
                    authenticationType = user.Identity?.AuthenticationType,
                    name = user.Identity?.Name
                },
                tokenHeader = tokenInfo,
                keyClaims = keyClaimsInfo,
                roles = roles,
                roleChecks = roleChecks,
                allClaims = claims,
                identities = identities,
                summary = new
                {
                    totalClaims = claims.Count,
                    totalRoles = roles.Count,
                    totalIdentities = identities.Count,
                    userId = keyClaimsInfo.nameIdentifier ?? keyClaimsInfo.sub
                }
            };

            logger.LogDebug("Extracted token information: {@TokenInfo}", result);
            return result;
        }
    }
}