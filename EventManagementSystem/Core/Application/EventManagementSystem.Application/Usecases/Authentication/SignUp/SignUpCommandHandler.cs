using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Usecases.Authentication.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result<AppUser>>
    {
        private readonly IAppUserService appUserService;
        private readonly ILogger<SignUpCommandHandler> logger;

        public SignUpCommandHandler(IAppUserService appUserService, ILogger<SignUpCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<Result<AppUser>> Handle(SignUpCommand command, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("SignUp attempt for Email: {Email}", command.Email);

            // Check if email already exists
            if (await this.appUserService.CheckEmailExists(command.Email))
            {
                logger.LogWarning("SignUp failed: Email already exists. Email: {Email}", command.Email);
                return Result<AppUser>.Failure("SignUp failed", null, 400, "Email already exists.");
            }

            // Create user
            string userId;
            try
            {
                userId = await this.appUserService.SignUpAsync(command.UserName , command.Email, command.Password);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SignUp failed for Email: {Email}", command.Email);
                return Result<AppUser>.Failure("SignUp failed", null, 500, ex.Message);
            }

            // Retrieve created user
            var user = await this.appUserService.GetUserAsync(userId);
            if (user == null)
            {
                logger.LogError("SignUp failed: User creation failed for Email: {Email}", command.Email);
                return Result<AppUser>.Failure("SignUp failed", null, 500, "User creation failed.");
            }

            // Set additional properties if needed (FirstName, LastName)
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PhoneNumber = command.PhoneNumber;

            // Optionally update user with these fields if your service supports it
            await this.appUserService.UpdateUserAsync(user.Id, user.UserName, user.FirstName, user.LastName, user.PhoneNumber);

            logger.LogInformation("SignUp successful for Email: {Email}", command.Email);
            return Result<AppUser>.Success("User created successfully.", user, 201);
        }
    }
}
