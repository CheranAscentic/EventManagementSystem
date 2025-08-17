namespace EventManagementSystem.Application.Usecases.RefreshToken
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class RefreshTokenCommand : IRequest<Result<CredentialsDTO>>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
