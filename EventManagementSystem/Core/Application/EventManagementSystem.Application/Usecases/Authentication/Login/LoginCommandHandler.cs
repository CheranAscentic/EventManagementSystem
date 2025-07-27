using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.Application.Usecases.Authentication.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, StandardResponseObject<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ILogger<LoginCommandHandler> logger;

        public LoginCommandHandler(IAppUserService appUserService, ILogger<LoginCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<StandardResponseObject<LoginDTO>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Attempting login for Email: {Email}", command.Email);

            var userData = await this.appUserService.LoginAsync(command.Email, command.Password);
            if (userData == null)
            {
                this.logger.LogWarning("Login failed for Email: {Email}", command.Email);
                return StandardResponseObject<AppUser>.BadRequest("Invalid credentials", "Login failed");
            }

            this.logger.LogInformation("Login successful for Email: {Email}", command.Email);
            return StandardResponseObject<AppUser>.Ok(userData, "Login successful");
        }
    }
}