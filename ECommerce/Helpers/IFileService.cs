namespace ECommerce.Helpers
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file, string folderName);
        Task<bool> DeleteFileAsync(string fileUrl);
        bool IsValidImage(IFormFile file);
        Task<FileUploadResult> UpdateFileAsync(IFormFile newFile, string oldFileUrl, string folderName);
    }
}
