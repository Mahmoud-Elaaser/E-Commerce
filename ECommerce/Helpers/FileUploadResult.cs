namespace ECommerce.Helpers
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string FileUrl { get; set; }
        public string ErrorMessage { get; set; }
        public string FileName { get; set; }
    }
}
