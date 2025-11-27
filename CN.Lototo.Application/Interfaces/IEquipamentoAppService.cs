using CN.Lototo.Application.Dto;

namespace CN.Lototo.Application.Interfaces;

public interface IEquipamentoAppService
{
    Task<List<EquipamentoDto>> GetByPlantAsync(int plantId, CancellationToken ct = default);

    Task<EquipamentoImportResultDto> ImportExcelAsync(
        int plantId,
        Stream excelStream,
        string fileName,
        string contentType,
        CancellationToken ct = default);

    Task CreateManualAsync(
        EquipamentoCreateManualRequestDto dto,
        Stream? imageStream,
        string? fileName,
        string? contentType,
        CancellationToken ct = default);

    Task UpdateAsync(EquipamentoUpdateDto dto, CancellationToken ct = default);

    Task UpdateImageAsync(
        Guid id,
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken ct = default);

    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
