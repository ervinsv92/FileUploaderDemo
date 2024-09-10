namespace FileUploaderDemo.Models
{
    public class FileUploadModel
    {
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}
