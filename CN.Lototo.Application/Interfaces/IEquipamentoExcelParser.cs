using CN.Lototo.Application.Dto;

namespace CN.Lototo.Application.Interfaces
{
    public interface IEquipamentoExcelParser
    {
        Task<ParsedEquipamentoFile> ParseAsync(Stream excelStream, CancellationToken ct = default);

    }
}
