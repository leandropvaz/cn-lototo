using CN.Lototo.Application.Dto;
using CN.Lototo.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

public class EquipmentExcelParser : IEquipamentoExcelParser
{
    public async Task<ParsedEquipamentoFile> ParseAsync(Stream excelStream, CancellationToken ct = default)
    {
        if (excelStream == null)
            throw new ArgumentNullException(nameof(excelStream));

        if (!excelStream.CanRead)
            throw new ArgumentException("Stream não está aberto para leitura.", nameof(excelStream));

        // EPPlus 8 – licença (modo non-commercial para desenvolvimento)
        ExcelPackage.License.SetNonCommercialPersonal("Leandro Vaz");

        excelStream.Position = 0;
        using var package = new ExcelPackage(excelStream);
        var ws = package.Workbook.Worksheets[0];

        // Cabeçalho do equipamento (ajustado ao template)
        var result = new ParsedEquipamentoFile
        {
            Tag = ws.Cells["D3"].GetValue<string>() ?? string.Empty,
            EquipmentName = (ws.Cells["F3"].GetValue<string>() ?? string.Empty).Trim(),

            // NOVO: fábrica e revisão (ajusta se na tua planilha for outra célula)
            FactoryName = (ws.Cells["J3"].GetValue<string>() ?? string.Empty).Trim(),
            RevisionInfo = (ws.Cells["K3"].GetValue<string>() ?? string.Empty).Trim()
        };

        // Mapa das colunas (1-based)
        const int startRow = 7;
        const int colNumero = 2;          // "N°"
        const int colTipoEnergia = 3;     // "Tipo de energia"
        const int colDescricao = 4;       // "Descrição da energia perigosa"
        const int colFoto = 5;            // Coluna E

        // GRUPO II – Controle de energias perigosas
        const int colIsoTag = 9;          // I - TAG dispositivo de isolamento
        const int colIsoLocation = 10;    // J - Localização dispositivo
        const int colIsoDescription = 11; // K - Dispositivo de isolamento
        const int colLockout = 12;        // L - Bloqueio

        // Faixa de colunas onde podem existir fotos flutuantes/shapes da foto
        const int colMinFloat = 4;        // E (0-based 4)
        const int colMaxFloat = 7;        // até H (0-based 7)

        // Fotos flutuantes (imagens "normais", não in-cell), na zona da coluna de foto
        var floatPictures = ws.Drawings
            .OfType<ExcelPicture>()
            .Where(p =>
                p.From != null &&
                p.From.Column >= colMinFloat &&
                p.From.Column <= colMaxFloat)
            .OrderBy(p => p.From.Row)
            .ToList();

        // Shapes com texto (balões de nota) também na zona da foto
        var shapes = ws.Drawings
            .OfType<ExcelShape>()
            .Where(s =>
                s.From != null &&
                s.From.Column >= colMinFloat &&
                s.From.Column <= colMaxFloat)
            .OrderBy(s => s.From.Row)
            .ToList();

        var currentRow = startRow;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            // se não houver número na coluna 2, assume fim da tabela
            var lineNumber = ws.Cells[currentRow, colNumero].GetValue<int?>();
            if (!lineNumber.HasValue)
                break;

            var energyType = ws.Cells[currentRow, colTipoEnergia].GetValue<string>() ?? string.Empty;
            var hazardDescription = ws.Cells[currentRow, colDescricao].GetValue<string>() ?? string.Empty;

            // GRUPO II – Controle de energias perigosas
            var isoTag = ws.Cells[currentRow, colIsoTag].GetValue<string>() ?? string.Empty;
            var isoLocation = ws.Cells[currentRow, colIsoLocation].GetValue<string>() ?? string.Empty;
            var isoDescription = ws.Cells[currentRow, colIsoDescription].GetValue<string>() ?? string.Empty;
            var lockout = ws.Cells[currentRow, colLockout].GetValue<string>() ?? string.Empty;

            byte[]? imageBytes = null;
            string? shapeNotes = null;

            // 1) Tenta primeiro IMAGEM "NA CÉLULA" (in-cell picture) na coluna Foto (E)
            try
            {
                var fotoCell = ws.Cells[currentRow, colFoto];

                // API nova no EPPlus 8: range.Picture
                if (fotoCell.Picture != null && fotoCell.Picture.Exists)
                {
                    var cellPic = fotoCell.Picture.Get();   // ExcelCellPicture
                    if (cellPic != null)
                    {
                        imageBytes = cellPic.GetImageBytes();
                    }
                }
            }
            catch
            {
                // Se der erro, ignora e cai no fallback
            }

            // 2) Tenta capturar texto dos shapes/balões da linha (se houver)
            var rowZeroBased = currentRow - 1;

            var shape = shapes.FirstOrDefault(s =>
                s.From != null &&
                s.From.Row <= rowZeroBased &&
                (s.To?.Row ?? s.From.Row) >= rowZeroBased);

            if (shape != null)
            {
                if (!string.IsNullOrWhiteSpace(shape.Text))
                {
                    shapeNotes = shape.Text.Trim();
                }
                else if (shape.RichText != null && shape.RichText.Count > 0)
                {
                    shapeNotes = string.Concat(shape.RichText.Select(rt => rt.Text)).Trim();
                }
            }

            // 3) Fallback: se não encontrou imagem "na célula",
            // tenta encontrar uma foto FLUTUANTE que cubra essa linha
            if (imageBytes == null && floatPictures.Count > 0)
            {
                var floatPic = floatPictures.FirstOrDefault(p =>
                    p.From != null &&
                    p.From.Row <= rowZeroBased &&
                    (p.To?.Row ?? p.From.Row) >= rowZeroBased);

                // Se nada "cobrir" a linha, usa o próximo disponível (sequencial)
                if (floatPic == null)
                {
                    floatPic = floatPictures.FirstOrDefault();
                }

                if (floatPic != null && floatPic.Image != null)
                {
                    imageBytes = floatPic.Image.ImageBytes;
                    // Remove da lista para não reutilizar a mesma imagem nas próximas linhas
                    floatPictures.Remove(floatPic);
                }
            }

            // 4) Adiciona linha ao resultado
            result.Rows.Add(new ParsedEquipamentoRow
            {
                LineNumber = lineNumber.Value,
                EnergyType = energyType,
                HazardDescription = hazardDescription,

                IsolationDeviceTag = string.IsNullOrWhiteSpace(isoTag) ? null : isoTag,
                IsolationDeviceLocation = string.IsNullOrWhiteSpace(isoLocation) ? null : isoLocation,
                IsolationDeviceDescription = string.IsNullOrWhiteSpace(isoDescription) ? null : isoDescription,
                LockoutType = string.IsNullOrWhiteSpace(lockout) ? null : lockout,

                ImageBytes = imageBytes,
                ShapeNotes = shapeNotes
            });

            currentRow++;
        }

        // Mantive async para bater com a interface
        return await Task.FromResult(result);
    }
}
