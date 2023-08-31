namespace Storage_Account_Blob_image_demo.Models
{
    public class Arquivo
    {
        public string Url { get; set; }
        public string FileOrImage { get; set; }
        public string ImageViewUpload { get; set; }
        
        public List<IFormFile> FileUpload { get; set; }
    }
}
