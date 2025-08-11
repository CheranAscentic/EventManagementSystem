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
        private readonly string serviceRoleKey;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Get configuration values
            var supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url configuration is missing");
            var supabaseKey = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key configuration is missing");
            this.serviceRoleKey = configuration["Supabase:ServiceRoleKey"] ?? throw new InvalidOperationException("Supabase:ServiceRoleKey configuration is missing");
            this.bucketName = configuration["Supabase:StorageBucket"] ?? "event-images";

            try
            {
                this.client = new Client(
                    supabaseUrl,
                    this.serviceRoleKey, // Use service role key for server-side operations
                    new SupabaseOptions
                    {
                        AutoConnectRealtime = false,
                    }
                );

                this.logger.LogInformation("Supabase client initialized successfully for bucket: {BucketName}", bucketName);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to initialize Supabase client");
                throw;
            }
        }

        public async Task<string> UploadEventImageAsync(Stream imageStream, string fileName, Guid eventId)
        {
            try
            {
                logger.LogInformation("Starting upload for file: {FileName} for event: {EventId}", fileName, eventId);

                // Create a unique file name to avoid conflicts
                var fileExtension = Path.GetExtension(fileName);
                var uniqueFileName = $"events/{eventId}/{Guid.NewGuid()}{fileExtension}";

                // Convert stream to byte array
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Upload to Supabase Storage with explicit options
                await client.Storage
                    .From(bucketName)
                    .Upload(fileBytes, uniqueFileName, new Supabase.Storage.FileOptions
                    {
                        Upsert = false, // Don't overwrite existing files
                        ContentType = GetContentType(fileExtension)
                    });

                // Get the public URL
                var publicUrl = client.Storage
                    .From(bucketName)
                    .GetPublicUrl(uniqueFileName);

                logger.LogInformation("File uploaded successfully. Public URL: {PublicUrl}", publicUrl);
                return publicUrl;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload file: {FileName} for event: {EventId}. Error: {Error}", fileName, eventId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteEventImageAsync(string fileName)
        {
            try
            {
                logger.LogInformation("Deleting file: {FileName}", fileName);

                await client.Storage
                    .From(bucketName)
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
                var publicUrl = client.Storage
                    .From(bucketName)
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
