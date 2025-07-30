namespace EventManagementSystem.Application.Usecases.EventCreation
{
    using MediatR;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;

    public class DeleteEventCommand : IRequest<Result<Event>>
    {
        public DeleteEventCommand(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; }
    }
}
