namespace EventManagementSystem.Application.Usecases.DeleteEvent
{
    using MediatR;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;

    public class DeleteEventCommand : IRequest<Result<Event>>
    {
        public DeleteEventCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
