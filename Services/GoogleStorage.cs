using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebAPI.Services
{
    public class GoogleStorage
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILogger<GoogleStorage> _logger;

        public GoogleStorage(IConfiguration configuration, ILogger<GoogleStorage> logger)
        {
            var bucket = configuration["GoogleCloudStorage:BucketName"];
            var credentialsPath = configuration["GoogleCloudStorage:CredentialsPath"];
            if (string.IsNullOrEmpty(bucket))
            {
                throw new ArgumentException("Google Cloud Storage bucket name is not configured.", nameof(configuration));
            }
            _bucketName = bucket;

            var credential = GoogleCredential.FromFile(credentialsPath);
            _storageClient = StorageClient.Create(credential);

            _logger = logger;
        }

        public async Task UploadEmptyObjectAsync(string folderPath)
        {
            using var stream = new MemoryStream(new byte[0]);
            _logger.LogInformation("Uploading empty object: {FolderPath}", folderPath);

            await _storageClient.UploadObjectAsync(_bucketName, folderPath, "application/x-www-form-urlencoded", stream);

            _logger.LogInformation("Uploaded empty object: {FolderPath}", folderPath);
        }

        public async Task CreateUserFolderAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/";
            using var stream = new MemoryStream(new byte[0]);

            _logger.LogInformation("[GoogleStorage] Start creating folder for user {UserId} - {Username}", userId, username);

            try
            {
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, stream);
                await CreateUserPlacesCsvAsync(userId, username);

                _logger.LogInformation("[GoogleStorage] Folder created successfully: {ObjectName}", objectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GoogleStorage] Error while creating folder for {UserId} - {Username}", userId, username);
                throw;
            }
        }

        public async Task CreateUserPlacesCsvAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/places.json";
            using var stream = new MemoryStream(new byte[0]);

            _logger.LogInformation("[GoogleStorage] Creating empty places.json for {UserId} - {Username}", userId, username);

            await _storageClient.UploadObjectAsync(_bucketName, objectName, "application/json", stream);

            _logger.LogInformation("[GoogleStorage] Created empty places.json for {UserId} - {Username}", userId, username);
        }

        public async Task AddUserPlaceToListFile(int userId, string username, string placeName)
        {
            var objectName = $"{userId}_{username}/places.json";
            string csvContent;

            _logger.LogInformation("[GoogleStorage] Adding place '{PlaceName}' to {ObjectName}", placeName, objectName);

            try
            {
                using var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                csvContent = await reader.ReadToEndAsync();

                _logger.LogInformation("[GoogleStorage] Downloaded existing places.json for {UserId} - {Username}", userId, username);
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                _logger.LogWarning("[GoogleStorage] places.json not found for {UserId} - {Username}, creating new one", userId, username);
                csvContent = "";
            }

            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var nextId = lines.Length + 1;

            csvContent += $"{nextId},{placeName}\n";

            var bytes = Encoding.UTF8.GetBytes(csvContent);
            using var uploadStream = new MemoryStream(bytes);

            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                "application/json",
                uploadStream
            );

            _logger.LogInformation("[GoogleStorage] Updated places.json for {UserId} - {Username} with new place {PlaceName}", userId, username, placeName);
        }
    }
}
