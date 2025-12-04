using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        const int colZeroEnergyVerification = 13; // M
        const int colTest = 14;

        // Faixa de colunas onde podem existir fotos flutuantes/shapes
        const int colMinFloat = 4;        // E (0-based 4)
        const int colMaxFloat = 7;        // até H (0-based 7)

        // Fotos flutuantes (imagens "normais", não in-cell)
        var floatPictures = ws.Drawings
            .OfType<ExcelPicture>()
            .Where(p =>
                p.From != null &&
                p.From.Column >= colMinFloat &&
                p.From.Column <= colMaxFloat)
            .OrderBy(p => p.From.Row)
            .ToList();

        // Shapes com texto (balões de nota)
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
            var zeroEnergyVerification = ws.Cells[currentRow, colZeroEnergyVerification].GetValue<string>() ?? string.Empty;
            var test = ws.Cells[currentRow, colTest].GetValue<string>() ?? string.Empty;

            byte[]? finalImageBytes = null;
            string? shapeNotes = null;

            var imagensLinha = new List<byte[]>();

            // 1) Imagem "na célula" (in-cell picture) na coluna Foto (E)
            try
            {
                var fotoCell = ws.Cells[currentRow, colFoto];

                if (fotoCell.Picture != null && fotoCell.Picture.Exists)
                {
                    var cellPic = fotoCell.Picture.Get();   // ExcelCellPicture
                    if (cellPic != null)
                    {
                        var bytes = cellPic.GetImageBytes();
                        if (bytes != null && bytes.Length > 0)
                            imagensLinha.Add(bytes);
                    }
                }
            }
            catch
            {
                // Se der erro, ignora e segue para as demais fontes
            }

            var rowZeroBased = currentRow - 1;

            // 2) Texto dos shapes / balões de nota da linha
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

            // 3) Fotos flutuantes que "cobrem" essa linha
            var picsForRow = floatPictures
                .Where(p =>
                    p.From != null &&
                    p.From.Row <= rowZeroBased &&
                    (p.To?.Row ?? p.From.Row) >= rowZeroBased)
                .ToList();

            foreach (var p in picsForRow)
            {
                if (p.Image?.ImageBytes is { Length: > 0 } bytes)
                {
                    imagensLinha.Add(bytes);
                }

                // remove da lista para não reutilizar a mesma imagem noutras linhas
                floatPictures.Remove(p);
            }

            // Se ainda não encontrou nada e sobrou alguma foto flutuante,
            // usa a próxima disponível (fallback)
            if (imagensLinha.Count == 0 && floatPictures.Count > 0)
            {
                var floatPic = floatPictures.First();
                if (floatPic.Image?.ImageBytes is { Length: > 0 } bytes)
                {
                    imagensLinha.Add(bytes);
                }
                floatPictures.Remove(floatPic);
            }

            // 4) Decide imagem final da linha (única ou mesclada)
            if (imagensLinha.Count == 1)
            {
                finalImageBytes = imagensLinha[0];
            }
            else if (imagensLinha.Count > 1)
            {
                finalImageBytes = MergeImagesVertical(imagensLinha);
            }

            // 5) Adiciona linha ao resultado
            result.Rows.Add(new ParsedEquipamentoRow
            {
                LineNumber = lineNumber.Value,
                EnergyType = energyType,
                HazardDescription = hazardDescription,

                IsolationDeviceTag = string.IsNullOrWhiteSpace(isoTag) ? null : isoTag,
                IsolationDeviceLocation = string.IsNullOrWhiteSpace(isoLocation) ? null : isoLocation,
                IsolationDeviceDescription = string.IsNullOrWhiteSpace(isoDescription) ? null : isoDescription,
                LockoutType = string.IsNullOrWhiteSpace(lockout) ? null : lockout,
                ZeroEnergyVerification = string.IsNullOrWhiteSpace(zeroEnergyVerification) ? null : zeroEnergyVerification,
                Test = string.IsNullOrWhiteSpace(test) ? null : test,

                ImageBytes = finalImageBytes,
                ShapeNotes = shapeNotes
            });

            currentRow++;
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Mescla várias imagens verticalmente, normalizando a largura
    /// e reduzindo o espaço em branco.
    /// </summary>
    private static byte[] MergeImagesVertical(IReadOnlyList<byte[]> imagensBytes)
    {
        if (imagensBytes == null || imagensBytes.Count == 0)
            return Array.Empty<byte>();

        const int padding = 10;

        var originalStreams = new List<MemoryStream>();
        var originalImages = new List<Image>();
        var resizedImages = new List<Image>();

        try
        {
            // Carrega todas as imagens a partir dos bytes
            foreach (var bytes in imagensBytes)
            {
                if (bytes == null || bytes.Length == 0)
                    continue;

                var ms = new MemoryStream(bytes);
                originalStreams.Add(ms);
                var img = Image.FromStream(ms);
                originalImages.Add(img);
            }

            if (originalImages.Count == 0)
                return Array.Empty<byte>();

            // Largura alvo: menor entre a maior largura das imagens e 1024 px
            var maxOrigWidth = originalImages.Max(i => i.Width);
            var targetWidth = Math.Min(maxOrigWidth, 1024);

            // Redimensiona tudo para a mesma largura, mantendo proporção
            foreach (var img in originalImages)
            {
                if (img.Width == targetWidth)
                {
                    resizedImages.Add((Image)img.Clone());
                }
                else
                {
                    var newHeight = (int)Math.Round(img.Height * (targetWidth / (double)img.Width));
                    var bmp = new Bitmap(targetWidth, newHeight);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(img, 0, 0, targetWidth, newHeight);
                    }
                    resizedImages.Add(bmp);
                }
            }

            var totalHeight = padding * (resizedImages.Count + 1) +
                              resizedImages.Sum(i => i.Height);

            var finalWidth = targetWidth + padding * 2;

            using var merged = new Bitmap(finalWidth, totalHeight);
            using (var g = Graphics.FromImage(merged))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var y = padding;
                foreach (var img in resizedImages)
                {
                    var x = padding;
                    g.DrawImage(img, x, y, img.Width, img.Height);
                    y += img.Height + padding;
                }
            }

            using var outStream = new MemoryStream();
            merged.Save(outStream, ImageFormat.Png);
            return outStream.ToArray();
        }
        finally
        {
            foreach (var img in originalImages)
                img.Dispose();

            foreach (var img in resizedImages)
                img.Dispose();

            foreach (var ms in originalStreams)
                ms.Dispose();
        }
    }
}
