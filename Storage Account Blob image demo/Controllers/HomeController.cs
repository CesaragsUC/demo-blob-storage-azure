using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Storage_Account_Blob_image_demo.Models;
using Storage_Account_Blob_image_demo.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;

namespace Storage_Account_Blob_image_demo.Controllers
{

    //Referencia: https://stackup.hashnode.dev/upload-files-to-azure-blob-storage-using-aspnet-core-web-api
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureBlobService _azureBlobService;
        private readonly string _azureBlobContainerName;
        private readonly string _azureBlobConnection;
        private readonly IConfiguration Configuration;

        public HomeController(ILogger<HomeController> logger,
            AzureBlobService azureBlobService,
            IConfiguration configuration)
        {
            _logger = logger;
            _azureBlobService = azureBlobService;
            Configuration = configuration;
            _azureBlobContainerName = Configuration["AzureConnections:FullImageContainerName"];
            _azureBlobConnection = Configuration["AzureConnections:AzureConnectionsString"];
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(Arquivo arquivo)
        {
            if (arquivo.FileUpload != null)
            {
                var response = await _azureBlobService.UploadFiles2(arquivo.FileUpload);
                return Ok(response);
            }

            return BadRequest("Ocorreu um erro ao tentar fazer o upload.");
        }

        [HttpGet]
        public async Task<ActionResult> GetAllBlobos()
        {
            var response = await _azureBlobService.GetUploadedBlobs();
            return Ok(response);
        }

        public async Task<BlobContainerClient> GetCloudBlobContainer(string containerName)
        {
            BlobServiceClient serviceClient = new BlobServiceClient(_azureBlobConnection);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;

        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            BlobContainerClient containerClient = await GetCloudBlobContainer(_azureBlobContainerName);
            var result = new List<FileData>();

            await foreach (Azure.Storage.Blobs.Models.BlobItem blobItem in containerClient.GetBlobsAsync())
            {

                var _file = new FileData();
                _file.FileType = GetFileType(blobItem);
                _file.Url = Flurl.Url.Combine(containerClient.Uri.AbsoluteUri, blobItem.Name);

                result.Add(_file);
            }

            Console.Out.WriteLine("Got Images");
            return View(result);
        }


        [HttpPost]
        public async Task<ActionResult> Post(string fileName)
        {

            Stream image = Request.Body;
            BlobContainerClient containerClient = await GetCloudBlobContainer(_azureBlobContainerName);
            string blobName = fileName.Replace("-", string.Empty);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(image);
            return Created(blobClient.Uri, null);
        }

        void teste()
        {

        }
           

        protected FileType GetFileType(BlobItem blobItem)
        { 
            switch(blobItem.Properties.ContentType)
            {
                case "image/jpeg":
                case "image/jpg":
                case "image/png":
                case "image/dng":
                    return FileType.Image;
                case "image/gif":
                    return FileType.Image;
                case "video/mp4":
                    return FileType.Video;
                case "audio/mp3":
                    return FileType.Audio;
                default:
                    return FileType.Document;   
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
