using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Lototo.Application.Dto
{
    public class EquipamentoDto
    {
        public Guid Id { get; set; }
        public int PlantId { get; set; }

        // Cabeçalho
        public string Tag { get; set; } = string.Empty;
        public string EquipmentName { get; set; } = string.Empty;
        public string? FactoryName { get; set; }
        public string? RevisionInfo { get; set; }

        // I. Identificação
        public int LineNumber { get; set; }
        public string EnergyType { get; set; } = string.Empty;
        public string HazardDescription { get; set; } = string.Empty;
        public string ZeroEnergyVerification { get; set; } = string.Empty;
        public string Test { get; set; } = string.Empty;

        // II. Controle
        public string? IsolationDeviceTag { get; set; }
        public string? IsolationDeviceLocation { get; set; }
        public string? IsolationDeviceDescription { get; set; }
        public string? LockoutType { get; set; }

        // Excel / imagem
        public string? SourceExcelBlobUrl { get; set; }
        public string? ImageBlobUrl { get; set; }
        public string? ImageNotes { get; set; }

        public bool IsDeleted { get; set; }
    }
}