namespace EventManagementSystem.Application.Usecases.EventCreation
{
    using System.Data.Common;
    using Microsoft.Data.SqlClient;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<Event>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UpdateEventCommandHandler> logger;
        private readonly IRepository<Event> repository;

        public UpdateEventCommandHandler(IRepository<Event> repository, IUnitOfWork unitOfWork, ILogger<UpdateEventCommandHandler> logger)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<Event>> Handle(UpdateEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Updating event: {EventId}", command.Id);
            await this.unitOfWork.BeginTransactionAsync();
            try
            {
                // Get the event from the database
                var eventEntity = await this.repository.GetAsync(command.Id);
                if (eventEntity == null)
                {
                    this.logger.LogWarning("Event not found: {EventId}", command.Id);
                    await this.unitOfWork.RollbackTransactionAsync();
                    return Result<Event>.Failure("Event not found.", null, 404, "Not Found");
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(command.Title))
                {
                    eventEntity.Title = command.Title;
                }

                if (!string.IsNullOrWhiteSpace(command.Description))
                {
                    eventEntity.Description = command.Description;
                }

                if (command.EventDate != null)
                {
                    eventEntity.EventDate = command.EventDate.Value;
                }

                if (!string.IsNullOrWhiteSpace(command.Location))
                {
                    eventEntity.Location = command.Location;
                }

                if (!string.IsNullOrWhiteSpace(command.Type))
                {
                    eventEntity.Type = command.Type;
                }

                if (command.Capacity != null)
                {
                    eventEntity.Capacity = command.Capacity.Value;
                }

                if (command.RegistrationCutoffDate != null)
                {
                    eventEntity.RegistrationCutoffDate = command.RegistrationCutoffDate.Value;
                }

                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                await this.unitOfWork.CommitTransactionAsync();

                this.logger.LogInformation("Event updated successfully: {EventId}", eventEntity.Id);
                return Result<Event>.Success("Event updated successfully.", eventEntity, 200);
            }
            catch (ObjectDisposedException ex)
            {
                this.logger.LogError(ex, "Object disposed while updating event: {EventId}", command.Id);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to update event.", null, 500, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                this.logger.LogError(ex, "Invalid operation while updating event: {EventId}", command.Id);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to update event.", null, 500, ex.Message);
            }
            catch (SqlException ex)
            {
                this.logger.LogError(ex, "SQL error while updating event: {EventId}", command.Id);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to update event.", null, 500, ex.Message);
            }
            catch (DbException ex)
            {
                this.logger.LogError(ex, "Database error while updating event: {EventId}", command.Id);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to update event.", null, 500, ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                this.logger.LogError(ex, "Operation canceled while updating event: {EventId}", command.Id);
                await this.unitOfWork.RollbackTransactionAsync();
                return Result<Event>.Failure("Failed to update event.", null, 500, ex.Message);
            }
        }
    }
}
