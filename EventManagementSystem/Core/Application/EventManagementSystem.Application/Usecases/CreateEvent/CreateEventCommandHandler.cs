namespace EventManagementSystem.Application.Usecases.EventCreation
{
    using System;
    using System.Data.Common;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.Interfaces;

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<Event>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CreateEventCommandHandler> logger;

        public CreateEventCommandHandler(IRepository<Event> eventRepository, IUnitOfWork unitOfWork, ILogger<CreateEventCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<Event>> Handle(CreateEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Creating event: {Title}", command.Title);
            await this.unitOfWork.BeginTransactionAsync();
            try
            {
                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = command.Title,
                    Description = command.Description,
                    EventDate = command.EventDate,
                    Location = command.Location,
                    Type = command.Type,
                    Capacity = command.Capacity,
                    RegistrationCutoffDate = command.RegistrationCutoffDate,
                    IsOpenForRegistration = true,
                };

                await this.eventRepository.AddAsync(newEvent);
                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                await this.unitOfWork.CommitTransactionAsync();

                this.logger.LogInformation("Event created successfully: {EventId}", newEvent.Id);
                return Result<Event>.Success("Event created successfully.", newEvent, 201);
            }
            catch (ObjectDisposedException ex)
            {
                this.logger.LogError(ex, "Object disposed while creating event: {Title}", command.Title);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to create event.", null, 500, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                this.logger.LogError(ex, "Invalid operation while creating event: {Title}", command.Title);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to create event.", null, 500, ex.Message);
            }
            catch (SqlException ex)
            {
                this.logger.LogError(ex, "SQL error while creating event: {Title}", command.Title);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to create event.", null, 500, ex.Message);
            }
            catch (DbException ex)
            {
                this.logger.LogError(ex, "Database error while creating event: {Title}", command.Title);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to create event.", null, 500, ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                this.logger.LogError(ex, "Operation canceled while creating event: {Title}", command.Title);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to create event.", null, 500, ex.Message);
            }
        }
    }
}
