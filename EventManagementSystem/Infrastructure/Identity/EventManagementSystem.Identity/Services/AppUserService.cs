namespace EventManagementSystem.Identity.Services
{
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;

    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AppUserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<string> SignUpAsync(string userName, string email, string password)
        {
            var user = new AppUser
            {
                UserName = userName,
                Email = email,
            };

            var result = await this.userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return user.Id.ToString();
            }

            throw new System.Exception(string.Join("; ", result.Errors));
        }

        public async Task<AppUser?> LoginAsync(string email, string password)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var signInResult = await this.signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);
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
    }
}
