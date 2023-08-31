namespace Storage_Account_Blob_image_demo.Models
{
    public class AzureConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string FullImageContainerName { get; set; }
        public string ThumbnailImageContainerName { get; set; }
        public string AzureConnectionsString { get; set; }
    }
}
