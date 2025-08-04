using EventManagementSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Interfaces
{
    public interface IAppUserService
    {
        Task<string> RegisterAsync(string userName, string email, string password, string role = "User");

        Task<AppUser> LoginAsync(string email, string password);

        Task<AppUser> GetUserAsync(string userId);

        Task<AppUser> DeleteUserAsync(string userId);

        Task<AppUser> UpdateUserAsync(Guid userId, string userName, string firstName, string lastName, string phoneNumber);

        Task<bool> CheckEmailExists(string email);

        /// <summary>
        /// Gets the primary role of a user.
        /// </summary>
        /// <param name="user">The user to get the role for</param>
        /// <returns>The user's primary role or empty string if no roles found</returns>
        Task<string> GetUserRoleAsync(AppUser user);

        /// <summary>
        /// Gets all roles of a user.
        /// </summary>
        /// <param name="user">The user to get roles for</param>
        /// <returns>List of user roles</returns>
        Task<IList<string>> GetUserRolesAsync(AppUser user);
    }
}
