using System.Numerics;

namespace CN.Lototo.Domain.Entities
{
    public class Equipamento
    {
        public Guid Id { get; set; }

        public int PlantaId { get; set; }
        public Plantas Planta { get; set; }

        // CABEÇALHO / EQUIPAMENTO
        public string Tag { get; set; } = string.Empty;           // Tag do equipamento (ex.: FMTZ-100-BC11)
        public string EquipmentName { get; set; } = string.Empty; // Nome / descrição
        public string? FactoryName { get; set; }                  // Fábrica (ex.: Matozinhos)
        public string? RevisionInfo { get; set; }                 // Data / Revisão (ex.: "Data elaboração: ... Revisão 1")

        // I. IDENTIFICAÇÃO DE ENERGIA PERIGOSA
        public int LineNumber { get; set; }                       // Nº (linha / risco)
        public string EnergyType { get; set; } = string.Empty;    // Tipo de energia perigosa / Magnitude
        public string HazardDescription { get; set; } = string.Empty; // Descrição da energia perigosa

        // II. CONTROLE DE ENERGIAS PERIGOSAS
        public string? IsolationDeviceTag { get; set; }           // TAG Equipamento Dispositivo de isolamento
        public string? IsolationDeviceLocation { get; set; }      // Localização dispositivo
        public string? IsolationDeviceDescription { get; set; }   // Dispositivo de isolamento
        public string? LockoutType { get; set; }                  // Bloqueio (ex.: "Sim, ICV")

        // EXCEL / IMAGEM
        public string? SourceExcelBlobUrl { get; set; }           // URL do Excel original
        public string? ImageBlobUrl { get; set; }                 // URL da imagem
        public string? ImageNotes { get; set; }                   // Notas OCR/shape

        // AUDITORIA / STATUS
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
