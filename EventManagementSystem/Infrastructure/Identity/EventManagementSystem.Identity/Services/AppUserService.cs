using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Identity.Services
{
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
                Email = email
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return user.Id;
            }
            throw new System.Exception(string.Join("; ", result.Errors));
        }

        public async Task<AppUser> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);
            if (signInResult.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<AppUser> GetUserAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<AppUser> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return user;
            }
            throw new System.Exception(string.Join("; ", result.Errors));
        }

        public async Task<AppUser> UpdateUserAsync(string userId, string userName, string firstName, string lastName, string phoneNumber)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return null;

            user.UserName = userName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }
            throw new System.Exception(string.Join("; ", result.Errors));
        }
    }
}
