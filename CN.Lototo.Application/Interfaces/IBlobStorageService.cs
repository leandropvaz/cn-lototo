namespace CN.Lototo.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(
            Stream stream,
            string containerName,
            string fileName,
            string contentType,
            CancellationToken ct = default);
    }
}
