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

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Get configuration values
            var supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url configuration is missing");
            var supabaseKey = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key configuration is missing");
            this.bucketName = configuration["Supabase:StorageBucket"] ?? "event-images";

            try
            {
                this.client = new Client(
                    supabaseUrl,
                    supabaseKey,
                    new SupabaseOptions
                    {
                        AutoConnectRealtime = true,
                    }
                );

                this.client.InitializeAsync();

                this.logger.LogInformation("Supabase client initialized successfully for bucket: {BucketName}", bucketName);

                //var bucket = client.Storage.GetBucket(bucketName);

                //if (bucket == null)
                //{
                //    this.logger.LogInformation("Creating bucket: {BucketName}", bucketName);
                //    client.Storage.CreateBucket(bucketName, new Supabase.Storage.BucketUpsertOptions
                //    {
                //        Public = true // Set to true if you want the files to be publicly accessible
                //    });

                //    this.logger.LogInformation("Bucket created successfully: {BucketName}", bucketName);
                //}

                //this.bucket = client.Storage.GetBucket(bucketName) ?? throw new ArgumentNullException(nameof(this.bucket));
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

                // Upload to Supabase Storage
                await client.Storage
                    .From(bucketName)
                    .Upload(fileBytes, uniqueFileName, new Supabase.Storage.FileOptions
                    {
                        Upsert = false // Don't overwrite existing files
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
                logger.LogError(ex, "Failed to upload file: {FileName} for event: {EventId}", fileName, eventId);
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
    }
}
