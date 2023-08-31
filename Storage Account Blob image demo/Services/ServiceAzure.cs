namespace Storage_Account_Blob_image_demo.Services
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;


    public class AzureBlobService
    {
        BlobServiceClient _blobClient;
        BlobContainerClient _containerClient;
        public string azureConnectionString;
        public AzureBlobService(string connectionString)
        {
            _blobClient = new BlobServiceClient(connectionString);
            _containerClient = _blobClient.GetBlobContainerClient("images");
            azureConnectionString = connectionString;
        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(List<IFormFile> files)
        {

            var azureResponse = new List<Azure.Response<BlobContentInfo>>();

            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    var client = await _containerClient.UploadBlobAsync(fileName, memoryStream, default);
                    
                    azureResponse.Add(client);
                }
            };

            return azureResponse;
        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles2(List<IFormFile> files)
        {

            var azureResponse = new List<Azure.Response<BlobContentInfo>>();

            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {

                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    var blob = _containerClient.GetBlobClient(fileName);

                    var blobHttpHeader = new BlobHttpHeaders();


                    switch (file.ContentType)
                    {
                        case "image/jpeg":
                            blobHttpHeader.ContentType = "image/jpeg";
                            break;
                        case "image/jpg":
                            blobHttpHeader.ContentType = "image/jpg";
                            break;
                        case "image/png":
                            blobHttpHeader.ContentType = "image/png";
                            break;
                        case "image/gif":
                            blobHttpHeader.ContentType = "image/gif";
                            break;
                        case "video/mp4":
                            blobHttpHeader.ContentType = "video/mp4";
                            break;
                        case "audio/mp3":
                            blobHttpHeader.ContentType = "audio/mp3";
                            break;
                        default:
                            blobHttpHeader.ContentType = "application/octet-stream";
                            break;
                    }

                    var uploadedBlob = await blob.UploadAsync(memoryStream, blobHttpHeader);
                    azureResponse.Add(uploadedBlob);
                }
            };

            return azureResponse;
        }

        public async Task<List<BlobItem>> GetUploadedBlobs()
        {
            var items = new List<BlobItem>();
            var uploadedFiles = _containerClient.GetBlobsAsync();
            await foreach (BlobItem file in uploadedFiles)
            {
                items.Add(file);
            }

            return items;
        }
    }

}
