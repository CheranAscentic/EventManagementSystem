namespace EventManagementSystem.Application.Usecases.Logout
{
    using System;
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class LogoutCommand : IRequest<Result<object>>
    {
        public Guid UserId { get; set; }
    }
}
