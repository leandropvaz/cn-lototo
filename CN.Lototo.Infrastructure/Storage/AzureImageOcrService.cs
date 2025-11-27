using CN.Lototo.Application.Interfaces;
namespace CN.Lototo.Infrastructure.Storage
{
    public class AzureImageOcrService : IImageOcrService
    {
        public Task<string?> ExtractNotesAsync(Stream imageStream, CancellationToken ct = default)
        {
            // TODO: implementar chamada ao Azure Cognitive Services / Vision.
            // Por enquanto devolve null ou string vazia.
            return Task.FromResult<string?>(null);
        }
    }
}
