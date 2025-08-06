namespace EventManagementSystem.Identity.Services
{
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AppUserService> logger;

        public AppUserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AppUserService> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public async Task<string> RegisterAsync(string userName, string email, string password, string role = "User")
        {
            this.logger.LogInformation("Creating user. Email: {Email}", email);
            var user = new AppUser
            {
                UserName = userName,
                Email = email,
            };

            var result = await this.userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, role);
                return user.Id.ToString();
            }

            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            logger.LogError("User registration failed for Email: {Email}. Errors: {Errors}", email, errorMessages);
            throw new System.Exception(errorMessages);
        }

        public async Task<AppUser?> LoginAsync(string email, string password)
        {
            logger.LogInformation("Login attempt. Email: {Email}", email);
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var signInResult = await this.signInManager.PasswordSignInAsync(user.UserName!, password, isPersistent: false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<AppUser?> GetUserAsync(string userId)
        {
            return await this.userManager.FindByIdAsync(userId);
        }

        public async Task<AppUser?> DeleteUserAsync(string userId)
        {
            logger.LogInformation("Deleting user. UserId: {UserId}", userId);
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var result = await this.userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception(string.Join("; ", result.Errors));
        }

        public async Task<AppUser?> UpdateUserAsync(Guid userId, string userName, string firstName, string lastName, string phoneNumber)
        {
            logger.LogInformation("Updating user. UserId: {UserId}", userId);
            var user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return null;
            }

            user.UserName = userName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            var result = await this.userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception(string.Join("; ", result.Errors));
        }

        public async Task<bool> CheckEmailExists(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            return user == null ? false : true;
        }

        /// <summary>
        /// Gets the primary role of a user.
        /// </summary>
        /// <param name="user">The user to get the role for</param>
        /// <returns>The user's primary role or empty string if no roles found</returns>
        public async Task<string> GetUserRoleAsync(AppUser user)
        {
            var roles = await this.userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Gets all roles of a user.
        /// </summary>
        /// <param name="user">The user to get roles for</param>
        /// <returns>List of user roles</returns>
        public async Task<IList<string>> GetUserRolesAsync(AppUser user)
        {
            return await this.userManager.GetRolesAsync(user);
        }
    }

    /// <summary>
    /// Service for accessing current user context from HTTP requests
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CurrentUserService> logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public Guid? GetCurrentUserId()
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return null;
            }

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim);
            return null;
        }

        public string? GetCurrentUserEmail()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public bool IsCurrentUserAdmin()
        {
            return httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }

        public bool IsAuthenticated()
        {
            return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
