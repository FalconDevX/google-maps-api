using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using WebAPI.DTOs;

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

        public async Task CreateUserFolderAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/";
            using var stream = new MemoryStream(new byte[0]);

            _logger.LogInformation("[GoogleStorage] Start creating folder for user {UserId} - {Username}", userId, username);

            try
            {
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, stream);
                await CreateUserPlacesJsonAsync(userId, username);

                _logger.LogInformation("[GoogleStorage] Folder created successfully: {ObjectName}", objectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GoogleStorage] Error while creating folder for {UserId} - {Username}", userId, username);
                throw;
            }
        }

        public async Task CreateUserPlacesJsonAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/places.json";
            using var stream = new MemoryStream(new byte[0]);

            _logger.LogInformation("[GoogleStorage] Creating empty places.json for {UserId} - {Username}", userId, username);

            await _storageClient.UploadObjectAsync(_bucketName, objectName, "application/json", stream);

            _logger.LogInformation("[GoogleStorage] Created empty places.json for {UserId} - {Username}", userId, username);
        }

        public async Task<bool> AddUserPlaceToListFileAsync(int userId, string username, UserSavedPlacesDto newPlace)
        {
            var objectName = $"{userId}_{username}/places.json";
            List<UserSavedPlacesDto> places;

            try
            {
                using var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var json = await reader.ReadToEndAsync();
                places = JsonSerializer.Deserialize<List<UserSavedPlacesDto>>(json) ?? new List<UserSavedPlacesDto>();
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                places = new List<UserSavedPlacesDto>();

                await CreateUserFolderAsync(userId, username);
            }

            if (places.Any(p => p.Title == newPlace.Title))
            {
                _logger.LogInformation("[GoogleStorage] Place '{PlaceName}' already exists in {ObjectName}", newPlace.Title, objectName);
                return false; 
            }

            places.Add(newPlace);

            var newJson = JsonSerializer.Serialize(places, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(newJson);

            using var uploadStream = new MemoryStream(bytes);
            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                "application/json",
                uploadStream
            );

            return true; 
        }


        public async Task<List<string>> GetUserPlacesFromFileAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/places.json";

            try
            {
                using var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var json = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return new List<string>();
            }
        }

        public async Task<bool> DeleteUserPlaceFromFileAsync(int userId, string username, string placeName)
        {
            var objectName = $"{userId}_{username}/places.json";
            List<string> places;

            try
            {
                using var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var json = await reader.ReadToEndAsync();
                places = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                _logger.LogWarning("[GoogleStorage] places.json not found for {UserId} - {Username}", userId, username);
                return false;
            }

            var removedPlace = places.Remove(placeName);

            if(!removedPlace)
            {
                _logger.LogInformation("[GoogleStorage] Place '{PlaceName}' not found in {ObjectName}", placeName, objectName);
                return false;
            }

            var newJson = JsonSerializer.Serialize(places);
            var bytes = Encoding.UTF8.GetBytes(newJson);

            using var uploadStream = new MemoryStream(bytes);

            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                "application/json",
                uploadStream
            );

            return true;
        }

    }
}
