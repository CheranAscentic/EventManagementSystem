namespace EventManagementSystem.Application.Usecases.AdminRegistration
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class AdminRegistrationCommandHandler : IRequestHandler<AdminRegistrationCommand, Result<AppUser>>
    {
        private readonly IAppUserService appUserService;
        private readonly ILogger<AdminRegistrationCommandHandler> logger;
        private readonly IUnitOfWork unitOfWork;

        public AdminRegistrationCommandHandler(
            IAppUserService appUserService,
            ILogger<AdminRegistrationCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            this.appUserService = appUserService;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<AppUser>> Handle(AdminRegistrationCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Admin registration attempt for Email: {Email}", command.Email);

            // Check if email already exists
            if (await this.appUserService.CheckEmailExists(command.Email))
            {
                this.logger.LogWarning("Admin registration failed: Email already exists. Email: {Email}", command.Email);
                return Result<AppUser>.Failure("Admin registration failed", null, 400, "Email already exists.");
            }

            // Create admin user
            string userId;
            try
            {
                userId = await this.appUserService.RegisterAsync(command.UserName, command.Email, command.Password, "Admin");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Admin registration failed for Email: {Email}", command.Email);
                return Result<AppUser>.Failure("Admin registration failed", null, 500, ex.Message);
            }

            // Retrieve created user
            var user = await this.appUserService.GetUserAsync(userId);
            if (user == null)
            {
                this.logger.LogError("Admin registration failed: User creation failed for Email: {Email}", command.Email);
                return Result<AppUser>.Failure("Admin registration failed", null, 500, "User creation failed.");
            }

            // Set additional properties if needed (FirstName, LastName)
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PhoneNumber = command.PhoneNumber;

            // Optionally update user with these fields if your service supports it
            await this.appUserService.UpdateUserAsync(user.Id, user.UserName, user.FirstName, user.LastName, user.PhoneNumber);

            this.logger.LogInformation("Admin registration successful for Email: {Email}", command.Email);
            return Result<AppUser>.Success("Admin user created successfully.", user, 201);
        }
    }
}
