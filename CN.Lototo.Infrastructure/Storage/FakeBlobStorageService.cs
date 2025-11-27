using CN.Lototo.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Lototo.Infrastructure.Storage
{
    public class FakeBlobStorageService : IBlobStorageService
    {
        public Task<string> UploadAsync(
            Stream stream,
            string containerName,
            string fileName,
            string contentType,
            CancellationToken ct = default)
        {
            // Aqui podes logar se quiser
            var fakeUrl = $"https://localhost/fakeblob/{containerName}/{fileName}";
            return Task.FromResult(fakeUrl);
        }
    }
}
