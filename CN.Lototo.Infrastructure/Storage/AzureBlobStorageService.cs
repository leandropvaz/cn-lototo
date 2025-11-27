using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CN.Lototo.Application.Interfaces;

namespace CN.Lototo.Infrastructure.Storage
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadAsync(
            Stream stream,
            string containerName,
            string fileName,
            string contentType,
            CancellationToken ct = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

            var blobClient = container.GetBlobClient(fileName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };

            await blobClient.UploadAsync(stream, options, ct);

            return blobClient.Uri.ToString();
        }
    }
}
