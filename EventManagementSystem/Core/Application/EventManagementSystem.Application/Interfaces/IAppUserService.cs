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
        Task<string> SignUpAsync(string userName, string email, string password);

        Task<AppUser> LoginAsync(string email, string password);

        Task<AppUser> GetUserAsync(string userId);

        Task<AppUser> DeleteUserAsync(string userId);

        Task<AppUser> UpdateUserAsync(Guid userId, string userName, string firstName, string lastName, string phoneNumber);

        Task<bool> CheckEmailExists(string email);

    }
}
