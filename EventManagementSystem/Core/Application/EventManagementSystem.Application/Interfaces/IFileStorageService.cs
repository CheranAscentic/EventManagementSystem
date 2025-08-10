namespace EventManagementSystem.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadEventImageAsync(Stream imageStream, string fileName, Guid eventId);

        Task<bool> DeleteEventImageAsync(string fileName);

        string GetPublicUrl(string fileName);
    }
}