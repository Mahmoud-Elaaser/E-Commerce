namespace ECommerce.Helpers
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public FileService(
            IWebHostEnvironment environment,
            ILogger<FileService> logger,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "File is not selected or is empty."
                    };

                // 1. Get folder path
                string folderPath = Path.Combine(_environment.WebRootPath, "File", folderName);

                // 2. Generate unique file name
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // 3. Get full file path
                string filePath = Path.Combine(folderPath, fileName);

                // 4. Ensure folder exists
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 5. Save file
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                // 6. Generate URL
                string fileUrl = GetFileUrl(fileName, folderName);

                return new FileUploadResult
                {
                    Success = true,
                    FileUrl = fileUrl,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred while uploading the file."
                };
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return false;

                // Parse URL to get file path
                var uri = new Uri(fileUrl);
                var relativePath = uri.LocalPath.TrimStart('/');

                // Remove "File/" from path if present
                if (relativePath.StartsWith("File/", StringComparison.OrdinalIgnoreCase))
                    relativePath = relativePath.Substring(5);

                // Get full path
                string fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
                return false;
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null) return false;

            // Check file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            // Check extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        public async Task<FileUploadResult> UpdateFileAsync(IFormFile newFile, string oldFileUrl, string folderName)
        {
            // Delete old file
            if (!string.IsNullOrEmpty(oldFileUrl))
                await DeleteFileAsync(oldFileUrl);

            // Upload new file
            return await UploadFileAsync(newFile, folderName);
        }

        private string GetFileUrl(string fileName, string folderName)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request != null)
            {
                return $"{request.Scheme}://{request.Host}/File/{folderName}/{fileName}";
            }

            // Fallback to configuration
            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:4171";
            return $"{baseUrl}/File/{folderName}/{fileName}";
        }
    }

}
