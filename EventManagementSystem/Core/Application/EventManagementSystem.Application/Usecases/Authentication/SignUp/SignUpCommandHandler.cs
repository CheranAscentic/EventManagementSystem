using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Usecases.Authentication.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, StandardResponseObject<AppUser>>
    {
        private readonly IAppUserService appUserService;

        public SignUpCommandHandler(IAppUserService appUserService)
        {
            this.appUserService = appUserService;
        }

        public async Task<StandardResponseObject<AppUser>> Handle(SignUpCommand command, CancellationToken cancellationToken = default)
        {
            // Check if email already exists
            if (await this.appUserService.CheckEmailExists(command.Email))
            {
                return StandardResponseObject<AppUser>.BadRequest("Email already exists.");
            }

            // Create user
            string userId;
            try
            {
                userId = await this.appUserService.SignUpAsync(command.UserName , command.Email, command.Password);
            }
            catch (Exception ex)
            {
                return StandardResponseObject<AppUser>.InternalError(ex.Message);
            }

            // Retrieve created user
            var user = await this.appUserService.GetUserAsync(userId);
            if (user == null)
            {
                return StandardResponseObject<AppUser>.InternalError("User creation failed.");
            }

            // Set additional properties if needed (FirstName, LastName)
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PhoneNumber = command.PhoneNumber;

            // Optionally update user with these fields if your service supports it
            await this.appUserService.UpdateUserAsync(user.Id, user.UserName, user.FirstName, user.LastName, user.PhoneNumber);

            return StandardResponseObject<AppUser>.Created(user, "User created successfully.");
        }
    }
}
