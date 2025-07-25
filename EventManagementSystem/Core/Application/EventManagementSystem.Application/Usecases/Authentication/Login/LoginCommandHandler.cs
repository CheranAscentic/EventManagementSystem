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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, StandardResponseObject<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ITokenService tokenService;

        public LoginCommandHandler(IAppUserService appUserService, ITokenService tokenService)
        {
            this.appUserService = appUserService;
            this.tokenService = tokenService;
        }

        public async Task<StandardResponseObject<LoginDTO>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            AppUser? userData = await this.appUserService.LoginAsync(command.Email, command.Password);

            if (userData == null)
            {
                return StandardResponseObject<LoginDTO>.BadRequest("Invalid credentials", "Login failed");
            }

            var userToken = this.tokenService.CreateToken(userData);

            LoginDTO loginDTO = new LoginDTO
            {
                Id = userData.Id,
                Email = userData.Email!,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Token = userToken,
                token
            };

            return StandardResponseObject<LoginDTO>.Ok(loginDTO, "Login successful");
        }
    }
}