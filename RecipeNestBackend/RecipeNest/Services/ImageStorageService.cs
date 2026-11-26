using Amazon.S3;
using Amazon.S3.Model;
using RecipeNest.Interfaces;

namespace RecipeNest.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public ImageStorageService(
            IAmazonS3 s3Client,
            IConfiguration configuration)
        {
            _s3Client = s3Client;

            _bucketName = configuration["CloudflareR2:BucketName"]
                ?? throw new InvalidOperationException(
                    "R2 bucket name is missing.");
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No image provided.");

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            string[] allowedExtensions =
                { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid image format.");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Image exceeds 5 MB.");

            var fileName = $"{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            Console.WriteLine($"Uploading to bucket: {_bucketName}");
            Console.WriteLine($"Object key: {request.Key}");

            System.Diagnostics.Debug.WriteLine("Attempting R2 upload");

            var response = await _s3Client.PutObjectAsync(request);

            System.Diagnostics.Debug.WriteLine($"R2 bucket: {_bucketName}");
            System.Diagnostics.Debug.WriteLine($"Uploaded image: {fileName}");
            System.Diagnostics.Debug.WriteLine($"Upload status: {response.HttpStatusCode}");

            return $"/images/{fileName}";
        }

        public async Task DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            if (!imagePath.StartsWith("/images/"))
                return;

            var fileName = Path.GetFileName(imagePath);

            await _s3Client.DeleteObjectAsync(
                new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                });
        }
    }
}