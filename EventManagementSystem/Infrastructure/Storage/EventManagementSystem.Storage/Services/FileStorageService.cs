namespace EventManagementSystem.Storage.Services
{
    using EventManagementSystem.Application.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Supabase;

    public class FileStorageService : IFileStorageService
    {
        private readonly Client client;
        private readonly ILogger<FileStorageService> logger;
        private readonly string bucketName;

        public FileStorageService(
            Client supabaseClient,
            IConfiguration configuration, 
            ILogger<FileStorageService> logger)
        {
            this.client = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Get bucket name from configuration
            this.bucketName = configuration["Supabase:StorageBucket"] ?? "event-images";

            this.logger.LogInformation("FileStorageService initialized with bucket: {BucketName}", this.bucketName);
        }

        public async Task<string> UploadEventImageAsync(Stream imageStream, string fileName, Guid eventId)
        {
            try
            {
                this.logger.LogInformation("Starting upload for file: {FileName} for event: {EventId}", fileName, eventId);

                // Create a unique file name to avoid conflicts
                var fileExtension = Path.GetExtension(fileName);
                var uniqueFileName = $"events/{eventId}/{Guid.NewGuid()}{fileExtension}";

                // Convert stream to byte array
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Upload to Supabase Storage with explicit options
                await this.client.Storage
                    .From(this.bucketName)
                    .Upload(fileBytes, uniqueFileName, new Supabase.Storage.FileOptions
                    {
                        Upsert = false, // Don't overwrite existing files
                        ContentType = GetContentType(fileExtension),
                    });

                // Get the public URL
                var publicUrl = this.client.Storage
                    .From(this.bucketName)
                    .GetPublicUrl(uniqueFileName);

                logger.LogInformation("File uploaded successfully. Public URL: {PublicUrl}", publicUrl);
                return publicUrl;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to upload file: {FileName} for event: {EventId}. Error: {Error}", fileName, eventId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteEventImageAsync(string fileName)
        {
            try
            {
                logger.LogInformation("Deleting file: {FileName}", fileName);

                await this.client.Storage
                    .From(this.bucketName)
                    .Remove(fileName);

                logger.LogInformation("File deleted successfully: {FileName}", fileName);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete file: {FileName}", fileName);
                return false;
            }
        }

        public string GetPublicUrl(string fileName)
        {
            try
            {
                var publicUrl = this.client.Storage
                    .From(this.bucketName)
                    .GetPublicUrl(fileName);

                return publicUrl;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get public URL for file: {FileName}", fileName);
                throw;
            }
        }

        private static string GetContentType(string fileExtension)
        {
            return fileExtension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
