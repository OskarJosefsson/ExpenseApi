using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Tesseract;

namespace ExpenseApi.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task<string> ExtractTextFromImageAsync(string imagePath);
    }

    public class ImageService : IImageService
    {

        private readonly BlobContainerClient _containerClient;
        private readonly string connectionString;
        private readonly string containerName;

        public ImageService(IConfiguration configuration)
        {
            connectionString = configuration["AzureBlobStorage:ConnectionString"];
            containerName = configuration["AzureBlobStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var blobClient = _containerClient.GetBlobClient(file.FileName);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType // e.g., "image/jpeg", "image/png"
            };

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = httpHeaders
            });

            var url = GetBlobSasUri(connectionString, containerName, file.FileName);
            return url;
        }


        public string GetBlobSasUri(string connectionString, string containerName, string blobName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri.ToString(); // Use this URL on the web
            }

            throw new InvalidOperationException("Cannot generate SAS URI. Make sure you're using a key-based connection string.");
        }
        public async Task<string> ExtractTextFromImageAsync(string imageUrl)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(imageUrl);

            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == null || !contentType.StartsWith("image/"))
            {
                throw new Exception($"Invalid content type: {contentType}");
            }

            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + GetExtensionFromContentType(contentType));
            await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                await response.Content.CopyToAsync(fs);
            }

            try
            {
                using var engine = new TesseractEngine(@"./tessdata", "swe", EngineMode.Default);
                using var img = Pix.LoadFromFile(tempFilePath);
                using var page = engine.Process(img);
                return page.GetText();
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        // Helper to get extension from content type
        private string GetExtensionFromContentType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/bmp" => ".bmp",
                "image/tiff" => ".tiff",
                _ => throw new Exception("Unsupported image type: " + contentType)
            };
        }


    }
}
