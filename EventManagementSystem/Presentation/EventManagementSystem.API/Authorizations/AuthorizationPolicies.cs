namespace EventManagementSystem.API.Authorizations
{
    /// <summary>
    /// Defines authorization policy constants for the Event Management System.
    /// </summary>
    public static class AuthorizationPolicies
    {
        /// <summary>
        /// Policy that requires the user to be authenticated (any role).
        /// </summary>
        public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";

        /// <summary>
        /// Policy that requires the user to have the Admin role.
        /// </summary>
        public const string RequireAdminRole = "RequireAdminRole";

        /// <summary>
        /// Policy that requires the user to have either User or Admin role.
        /// </summary>
        public const string RequireUserOrAdminRole = "RequireUserOrAdminRole";

        /// <summary>
        /// Policy that allows access to the resource owner or admins.
        /// Used for scenarios where users can access their own data or admins can access any data.
        /// </summary>
        public const string RequireResourceOwnerOrAdmin = "RequireResourceOwnerOrAdmin";

        /// <summary>
        /// Policy for debugging JWT tokens. Logs detailed token information and allows access to authenticated users.
        /// Should only be enabled in development environments.
        /// </summary>
        public const string DebugToken = "DebugToken";
    }
}
