using CN.Lototo.Application.Dto;
using CN.Lototo.Application.Interfaces;
using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Interfaces;

namespace CN.Lototo.Application.Services;

public class EquipamentoAppService : IEquipamentoAppService
{
    private readonly IEquipamentoRepository _equipmentRepository;
    private readonly IBlobStorageService _blobStorage;
    private readonly IEquipamentoExcelParser _excelParser;
    private readonly IImageOcrService _ocrService;

    private const string ExcelContainer = "excel";
    private const string ImagesContainer = "imagens";

    public EquipamentoAppService(
        IEquipamentoRepository equipmentRepository,
        IBlobStorageService blobStorage,
        IEquipamentoExcelParser excelParser,
        IImageOcrService ocrService)
    {
        _equipmentRepository = equipmentRepository;
        _blobStorage = blobStorage;
        _excelParser = excelParser;
        _ocrService = ocrService;
    }

    public async Task<List<EquipamentoDto>> GetByPlantAsync(int plantId, CancellationToken ct = default)
    {
        var list = await _equipmentRepository.GetByPlantAsync(plantId, ct);
        return list.Select(MapToDto).ToList();
    }

    public async Task<EquipamentoImportResultDto> ImportExcelAsync(
        int plantId,
        Stream excelStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        if (excelStream == null || excelStream.Length == 0)
        {
            return new EquipamentoImportResultDto
            {
                Success = false,
                ErrorMessage = "Ficheiro vazio."
            };
        }

        excelStream.Position = 0;

        // 1) Guardar Excel no Blob
        var excelUrl = await _blobStorage.UploadAsync(
            excelStream,
            ExcelContainer,
            $"{Guid.NewGuid()}-{fileName}",
            contentType,
            ct);

        // 2) Volta pro início pra fazer o parse
        excelStream.Position = 0;
        var parsed = await _excelParser.ParseAsync(excelStream, ct);

        var records = new List<Equipamento>();

        // 3) Criar registros linha a linha, subindo imagem + OCR por registro
        foreach (var row in parsed.Rows)
        {
            var entity = new Equipamento
            {
                Id = Guid.NewGuid(),
                PlantaId = plantId,

                // Cabeçalho
                Tag = parsed.Tag,
                EquipmentName = parsed.EquipmentName,
                FactoryName = parsed.FactoryName,
                RevisionInfo = parsed.RevisionInfo,

                // I. Identificação
                LineNumber = row.LineNumber,
                EnergyType = row.EnergyType,
                HazardDescription = row.HazardDescription,

                // II. Controle
                IsolationDeviceTag = row.IsolationDeviceTag,
                IsolationDeviceLocation = row.IsolationDeviceLocation,
                IsolationDeviceDescription = row.IsolationDeviceDescription,
                LockoutType = row.LockoutType,
                Test = row.Test,
                ZeroEnergyVerification = row.ZeroEnergyVerification,

                SourceExcelBlobUrl = excelUrl,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                ImageNotes = row.ShapeNotes
            };

            // imagem específica dessa linha
            if (row.ImageBytes is { Length: > 0 })
            {
                using var imgStream = new MemoryStream(row.ImageBytes);

                var imageBlobName = $"{entity.Id}-L{row.LineNumber}.png";

                entity.ImageBlobUrl = await _blobStorage.UploadAsync(
                    imgStream,
                    ImagesContainer,
                    imageBlobName,
                    "image/png",
                    ct);

                imgStream.Position = 0;
                // Se quiseres OCR linha a linha em vez do shape:
                // var notes = await _ocrService.ExtractNotesAsync(imgStream, ct);
                // if (!string.IsNullOrEmpty(notes))
                //     entity.ImageNotes = notes;
            }

            records.Add(entity);
        }

        await _equipmentRepository.AddRangeAsync(records, ct);
        await _equipmentRepository.SaveChangesAsync(ct);

        return new EquipamentoImportResultDto
        {
            Success = true,
            ImportedCount = records.Count
        };
    }


    public async Task CreateManualAsync(
        EquipamentoCreateManualRequestDto dto,
        Stream? imageStream,
        string? fileName,
        string? contentType,
        CancellationToken ct = default)
    {
        string? imageUrl = null;

        if (imageStream != null && imageStream.Length > 0 && !string.IsNullOrEmpty(fileName))
        {
            imageStream.Position = 0;

            imageUrl = await _blobStorage.UploadAsync(
                imageStream,
                ImagesContainer,
                $"{Guid.NewGuid()}-{fileName}",
                contentType ?? "application/octet-stream",
                ct);

            imageStream.Position = 0;
            if (string.IsNullOrEmpty(dto.ImageNotes))
            {
                dto.ImageNotes = await _ocrService.ExtractNotesAsync(imageStream, ct);
            }
        }

        var entity = new Equipamento
        {
            Id = Guid.NewGuid(),
            PlantaId = dto.PlantId,

            // Cabeçalho
            Tag = dto.Tag,
            EquipmentName = dto.EquipmentName,
            FactoryName = dto.FactoryName,
            RevisionInfo = dto.RevisionInfo,

            // I. Identificação
            LineNumber = dto.LineNumber,
            EnergyType = dto.EnergyType,
            HazardDescription = dto.HazardDescription,

            // II. Controle
            IsolationDeviceTag = dto.IsolationDeviceTag,
            IsolationDeviceLocation = dto.IsolationDeviceLocation,
            IsolationDeviceDescription = dto.IsolationDeviceDescription,
            LockoutType = dto.LockoutType,
            ZeroEnergyVerification = dto.ZeroEnergyVerification,
            Test = dto.Test,

            ImageBlobUrl = imageUrl,
            ImageNotes = dto.ImageNotes,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _equipmentRepository.AddAsync(entity, ct);
        await _equipmentRepository.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(EquipamentoUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _equipmentRepository.GetByIdAsync(dto.Id, ct);
        if (entity == null) return;

        // Cabeçalho
        entity.Tag = dto.Tag;
        entity.EquipmentName = dto.EquipmentName;
        entity.FactoryName = dto.FactoryName;
        entity.RevisionInfo = dto.RevisionInfo;

        // I. Identificação
        entity.LineNumber = dto.LineNumber;
        entity.EnergyType = dto.EnergyType;
        entity.HazardDescription = dto.HazardDescription;

        // II. Controle
        entity.IsolationDeviceTag = dto.IsolationDeviceTag;
        entity.IsolationDeviceLocation = dto.IsolationDeviceLocation;
        entity.IsolationDeviceDescription = dto.IsolationDeviceDescription;
        entity.LockoutType = dto.LockoutType;
        entity.Test = dto.Test;
        entity.ZeroEnergyVerification = dto.ZeroEnergyVerification;

        entity.ImageNotes = dto.ImageNotes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _equipmentRepository.UpdateAsync(entity, ct);
        await _equipmentRepository.SaveChangesAsync(ct);
    }

    public async Task UpdateImageAsync(
        Guid id,
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        var entity = await _equipmentRepository.GetByIdAsync(id, ct);
        if (entity == null) return;

        imageStream.Position = 0;

        var imageUrl = await _blobStorage.UploadAsync(
            imageStream,
            ImagesContainer,
            $"{Guid.NewGuid()}-{fileName}",
            contentType,
            ct);

        imageStream.Position = 0;
        var notes = await _ocrService.ExtractNotesAsync(imageStream, ct);

        entity.ImageBlobUrl = imageUrl;
        if (!string.IsNullOrEmpty(notes))
            entity.ImageNotes = notes;

        entity.UpdatedAt = DateTime.UtcNow;

        await _equipmentRepository.UpdateAsync(entity, ct);
        await _equipmentRepository.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _equipmentRepository.GetByIdAsync(id, ct);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _equipmentRepository.UpdateAsync(entity, ct);
        await _equipmentRepository.SaveChangesAsync(ct);
    }

    private static EquipamentoDto MapToDto(Equipamento x) => new()
    {
        Id = x.Id,
        PlantId = x.PlantaId,

        // Cabeçalho
        Tag = x.Tag,
        EquipmentName = x.EquipmentName,
        FactoryName = x.FactoryName,
        RevisionInfo = x.RevisionInfo,

        // I. Identificação
        LineNumber = x.LineNumber,
        EnergyType = x.EnergyType,
        HazardDescription = x.HazardDescription,

        // II. Controle
        IsolationDeviceTag = x.IsolationDeviceTag,
        IsolationDeviceLocation = x.IsolationDeviceLocation,
        IsolationDeviceDescription = x.IsolationDeviceDescription,
        LockoutType = x.LockoutType,
        ZeroEnergyVerification = x.ZeroEnergyVerification,
        Test = x.Test,

        SourceExcelBlobUrl = x.SourceExcelBlobUrl,
        ImageBlobUrl = x.ImageBlobUrl,
        ImageNotes = x.ImageNotes,
        IsDeleted = x.IsDeleted
    };
}
