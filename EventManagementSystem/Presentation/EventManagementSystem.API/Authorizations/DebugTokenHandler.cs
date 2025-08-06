namespace EventManagementSystem.API.Authorizations
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Authorization handler for debugging JWT tokens.
    /// Logs comprehensive token information and allows access to any authenticated user.
    /// </summary>
    public class DebugTokenHandler : AuthorizationHandler<DebugTokenRequirement>
    {
        private readonly ILogger<DebugTokenHandler> logger;

        public DebugTokenHandler(ILogger<DebugTokenHandler> logger)
        {
            this.logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DebugTokenRequirement requirement)
        {
            var user = context.User;

            this.logger.LogInformation("=== DebugTokenHandler: JWT Token Analysis ===");

            // Log basic authentication status
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
            this.logger.LogInformation("User Authentication Status: {IsAuthenticated}", isAuthenticated);
            this.logger.LogInformation("Authentication Type: {AuthenticationType}", user.Identity?.AuthenticationType ?? "null");
            this.logger.LogInformation("User Name: {UserName}", user.Identity?.Name ?? "null");

            if (!isAuthenticated)
            {
                this.logger.LogWarning("DebugTokenHandler: User is not authenticated - access denied");
                context.Fail();
                return Task.CompletedTask;
            }

            // Log all claims in detail
            this.logger.LogInformation("=== JWT Claims Analysis ===");
            this.logger.LogInformation("Total Claims Count: {ClaimsCount}", user.Claims.Count());

            foreach (var claim in user.Claims)
            {
                this.logger.LogInformation("Claim: Type='{ClaimType}', Value='{ClaimValue}', ValueType='{ValueType}', Issuer='{Issuer}'", 
                    claim.Type, claim.Value, claim.ValueType, claim.Issuer);
            }

            // Log specific important claims
            var specificClaims = new Dictionary<string, string?>
            {
                ["NameIdentifier"] = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ["Email"] = user.FindFirst(ClaimTypes.Email)?.Value,
                ["Name"] = user.FindFirst(ClaimTypes.Name)?.Value,
                ["Sub"] = user.FindFirst("sub")?.Value,
                ["Exp"] = user.FindFirst("exp")?.Value,
                ["Iss"] = user.FindFirst("iss")?.Value,
                ["Aud"] = user.FindFirst("aud")?.Value
            };

            this.logger.LogInformation("=== Key Claims Summary ===");
            foreach (var kvp in specificClaims)
            {
                this.logger.LogInformation("  {ClaimName}: {ClaimValue}", kvp.Key, kvp.Value ?? "null");
            }

            // Log roles in detail
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            this.logger.LogInformation("=== Roles Analysis ===");
            this.logger.LogInformation("Total Roles Count: {RolesCount}", roles.Count);
            
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    this.logger.LogInformation("Role: {Role}", role);
                }
            }
            else
            {
                this.logger.LogWarning("No roles found for user");
            }

            // Test role checks
            this.logger.LogInformation("=== Role Verification Tests ===");
            this.logger.LogInformation("IsInRole('Admin'): {IsAdmin}", user.IsInRole("Admin"));
            this.logger.LogInformation("IsInRole('User'): {IsUser}", user.IsInRole("User"));

            // Log all identities
            this.logger.LogInformation("=== Identities Analysis ===");
            this.logger.LogInformation("Total Identities Count: {IdentitiesCount}", user.Identities.Count());
            
            foreach (var identity in user.Identities)
            {
                this.logger.LogInformation("Identity: Name='{IdentityName}', AuthType='{AuthType}', IsAuth={IsAuth}, Claims={ClaimsCount}",
                    identity.Name ?? "null", 
                    identity.AuthenticationType ?? "null", 
                    identity.IsAuthenticated,
                    identity.Claims.Count());
            }

            // Log token expiration info if available
            var expClaim = user.FindFirst("exp")?.Value;
            if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var exp))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                var timeUntilExpiry = expirationTime - DateTimeOffset.UtcNow;
                
                this.logger.LogInformation("=== Token Expiration Info ===");
                this.logger.LogInformation("Token Expires At: {ExpirationTime:yyyy-MM-dd HH:mm:ss} UTC", expirationTime);
                this.logger.LogInformation("Time Until Expiry: {TimeUntilExpiry}", timeUntilExpiry);
                this.logger.LogInformation("Is Token Expired: {IsExpired}", timeUntilExpiry.TotalSeconds <= 0);
            }

            this.logger.LogInformation("=== End JWT Token Analysis ===");

            // Allow access for any authenticated user (this is a debug endpoint)
            this.logger.LogInformation("DebugTokenHandler: User is authenticated - access granted for debugging");
            context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Authorization requirement for debugging JWT tokens
    /// </summary>
    public class DebugTokenRequirement : IAuthorizationRequirement
    {
        // This requirement is satisfied if the user is authenticated
        // The handler will log detailed information about the JWT token
    }
}