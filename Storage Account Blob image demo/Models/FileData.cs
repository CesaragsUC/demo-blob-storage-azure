using System.ComponentModel;

namespace Storage_Account_Blob_image_demo.Models
{
    public class FileData
    {
        public string Url { get; set; }
        public FileType FileType { get; set; }

    }

    public enum FileType
    {
        [Description("Imagem")]
        Image,
        [Description("Video")]
        Video,
        [Description("Audio")]
        Audio,
        [Description("Document")]
        Document
    }
}
