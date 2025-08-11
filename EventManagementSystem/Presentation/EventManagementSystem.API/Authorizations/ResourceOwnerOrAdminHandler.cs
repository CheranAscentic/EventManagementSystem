namespace EventManagementSystem.API.Authorizations
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Authorization handler for resource owner or admin access.
    /// This handler allows access if the user is an admin or if they are accessing their own resources.
    /// </summary>
    public class ResourceOwnerOrAdminHandler : AuthorizationHandler<ResourceOwnerOrAdminRequirement>
    {
        private readonly ILogger<ResourceOwnerOrAdminHandler> logger;

        public ResourceOwnerOrAdminHandler(ILogger<ResourceOwnerOrAdminHandler> logger)
        {
            this.logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceOwnerOrAdminRequirement requirement)
        {
            var user = context.User;

            // Log comprehensive user information
            LogUserDetails(user);

            // Check if user is an Admin
            if (user.IsInRole("Admin"))
            {
                this.logger.LogInformation("ResourceOwnerOrAdminHandler: User has Admin role - access granted");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check if user is authenticated (required for resource owner check)
            if (!user.Identity?.IsAuthenticated == true)
            {
                this.logger.LogWarning("ResourceOwnerOrAdminHandler: User is not authenticated - access denied");
                context.Fail();
                return Task.CompletedTask;
            }

            // For resource owner check, we need access to the specific resource
            // This is handled in the command handlers with CurrentUserService
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                this.logger.LogInformation("ResourceOwnerOrAdminHandler: User is authenticated with valid UserId - access granted for resource owner check");
                context.Succeed(requirement);
            }
            else
            {
                this.logger.LogWarning("ResourceOwnerOrAdminHandler: User is authenticated but UserId claim is missing - access denied");
                context.Fail();
            }

            return Task.CompletedTask;
        }

        private void LogUserDetails(ClaimsPrincipal user)
        {
            this.logger.LogDebug("=== ResourceOwnerOrAdminHandler: Complete User Details ===");
            
            // Log basic identity information
            this.logger.LogDebug("User.Identity.IsAuthenticated: {IsAuthenticated}", user.Identity?.IsAuthenticated ?? false);
            this.logger.LogDebug("User.Identity.Name: {Name}", user.Identity?.Name ?? "null");
            this.logger.LogDebug("User.Identity.AuthenticationType: {AuthenticationType}", user.Identity?.AuthenticationType ?? "null");

            // Log all claims
            this.logger.LogDebug("User Claims Count: {ClaimsCount}", user.Claims.Count());
            foreach (var claim in user.Claims)
            {
                this.logger.LogDebug("Claim - Type: {ClaimType}, Value: {ClaimValue}, ValueType: {ValueType}, Issuer: {Issuer}", 
                    claim.Type, claim.Value, claim.ValueType, claim.Issuer);
            }

            // Log specific important claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = user.FindFirst(ClaimTypes.Name)?.Value;
            var subClaim = user.FindFirst("sub")?.Value;
            var jwtEmailClaim = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            this.logger.LogDebug("Key Claims Summary:");
            this.logger.LogDebug("  - NameIdentifier (UserId): {UserId}", userIdClaim ?? "null");
            this.logger.LogDebug("  - Email: {Email}", emailClaim ?? "null");
            this.logger.LogDebug("  - Name: {Name}", nameClaim ?? "null");
            this.logger.LogDebug("  - Sub: {Sub}", subClaim ?? "null");
            this.logger.LogDebug("  - JWT Email: {JwtEmail}", jwtEmailClaim ?? "null");

            // Log all roles
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            this.logger.LogDebug("User Roles Count: {RolesCount}", roles.Count);
            foreach (var role in roles)
            {
                this.logger.LogDebug("Role: {Role}", role);
            }

            // Check specific role queries
            this.logger.LogDebug("IsInRole('Admin'): {IsAdmin}", user.IsInRole("Admin"));
            this.logger.LogDebug("IsInRole('User'): {IsUser}", user.IsInRole("User"));

            // Log all identities if multiple
            this.logger.LogDebug("Identities Count: {IdentitiesCount}", user.Identities.Count());
            foreach (var identity in user.Identities)
            {
                this.logger.LogDebug("Identity - Name: {IdentityName}, AuthenticationType: {AuthType}, IsAuthenticated: {IsAuth}",
                    identity.Name ?? "null", identity.AuthenticationType ?? "null", identity.IsAuthenticated);
            }

            this.logger.LogDebug("=== End User Details ===");
        }
    }

    /// <summary>
    /// Authorization requirement for resource owner or admin access.
    /// </summary>
    public class ResourceOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}