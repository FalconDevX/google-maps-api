using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.CodeDom;
using System.IO;
using System.Text;

namespace WebAPI.Services
{
    public class GoogleStorage
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public GoogleStorage(IConfiguration configuration)
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

        }

        public async Task UploadEmptyObjectAsync(string folderPath)
        {
            using var stream = new MemoryStream(new byte[0]);
            await _storageClient.UploadObjectAsync(_bucketName, folderPath, "application/x-www-form-urlencoded", stream);
        }

        public async Task CreateUserFolderAsync(int userId, string username)
        {
            Console.WriteLine($"[GoogleStorage] Start tworzenia folderu dla {userId} - {username}");

            var objectName = $"{userId}_{username}/";
            using var stream = new MemoryStream(new byte[0]);

            await _storageClient.UploadObjectAsync("maps-api-storage", objectName, null, stream);

            await CreateUserPlacesCsvAsync(userId, username);

            Console.WriteLine($"[GoogleStorage] Utworzono folder: {objectName}");
        }

        public async Task CreateUserPlacesCsvAsync(int userId, string username)
        {
            var objectName = $"{userId}_{username}/places.csv";
            using var stream = new MemoryStream(new byte[0]);
            await _storageClient.UploadObjectAsync(_bucketName, objectName, "text/csv", stream);
        }

        public async Task AddUserPlaceToListFile(int userId, string username, string placeName)
        {
            var objectName = $"{userId}_{username}/places.csv";
            string csvContent;

            try
            {
                using var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                csvContent = await reader.ReadToEndAsync();
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
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
                "text/csv",
                uploadStream
            );
        }
    }
}
