namespace EventManagementSystem.Domain.Models
{
    using System;
    using EventManagementSystem.Domain.Interfaces;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Represents an application user with additional properties beyond the standard Identity user.
    /// 
    /// Example usage of userRole functionality:
    /// 
    /// 1. Using ToDto() with role:
    ///    var userRole = await appUserService.GetUserRoleAsync(user);
    ///    var userDto = user.ToDto(userRole);
    /// 
    /// 2. Using default ToDto():
    ///    var userDto = user.ToDto(); // userRole will be empty string
    /// 
    /// 3. Getting all user roles:
    ///    var allRoles = await appUserService.GetUserRolesAsync(user);
    /// </summary>
    public class AppUser : IdentityUser<Guid>, HasDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public object ToDto()
        {
            return new UserDTO
            {
                UserId = this.Id,
                Email = this.Email ?? string.Empty,
                UserName = this.UserName ?? string.Empty,
                FirstName = this.FirstName ?? string.Empty,
                LastName = this.LastName ?? string.Empty,
                userRole = string.Empty, // Default empty role when not specified
                PhoneNumber = this.PhoneNumber ?? string.Empty,
            };
        }

        /// <summary>
        /// Converts the AppUser to UserDTO with the specified role.
        /// </summary>
        /// <param name="userRole">The user's role</param>
        /// <returns>UserDTO with role information</returns>
        public UserDTO ToDto(string userRole)
        {
            return new UserDTO
            {
                UserId = this.Id,
                Email = this.Email ?? string.Empty,
                UserName = this.UserName ?? string.Empty,
                FirstName = this.FirstName ?? string.Empty,
                LastName = this.LastName ?? string.Empty,
                userRole = userRole ?? string.Empty,
                PhoneNumber = this.PhoneNumber ?? string.Empty,
            };
        }

        public class UserDTO : IsDto
        {
            public Guid UserId { get; set; }

            public string Email { get; set; } = string.Empty;

            public string UserName { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string userRole { get; set; } = string.Empty;

            public string PhoneNumber { get; set; } = string.Empty;

        }
    }
}
