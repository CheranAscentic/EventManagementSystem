using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Usecases.Authentication.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, StandardResponseObject<AppUser>>
    {
        private readonly IAppUserService appUserService;

        public LoginCommandHandler(IAppUserService appUserService)
        {
            this.appUserService = appUserService;
        }

        public async Task<StandardResponseObject<AppUser>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var userData = await this.appUserService.LoginAsync(command.Email, command.Password);
            if (userData == null)
            {
                return StandardResponseObject<AppUser>.BadRequest("Invalid credentials", "Login failed");
            }

            return StandardResponseObject<AppUser>.Ok(userData, "Login successful");
        }
    }
}