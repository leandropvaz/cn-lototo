using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Lototo.Application.Dto
{
    public class ParsedEquipamentoRow
    {
        // I. Identificação de energia perigosa
        public int LineNumber { get; set; }
        public string EnergyType { get; set; } = string.Empty;
        public string HazardDescription { get; set; } = string.Empty;

        // II. Controle de energias perigosas (novos)
        public string? IsolationDeviceTag { get; set; }          // TAG disp. isolamento
        public string? IsolationDeviceLocation { get; set; }     // Localização disp.
        public string? IsolationDeviceDescription { get; set; }  // Dispositivo
        public string? LockoutType { get; set; }                 // Bloqueio

        public string ZeroEnergyVerification { get; set; } = string.Empty;
        public string Test { get; set; } = string.Empty;

        // Já existiam
        public byte[]? ImageBytes { get; set; }
        public string? ShapeNotes { get; set; }
    }
    public class ParsedEquipamentoFile
    {
        // Cabeçalho do equipamento
        public string Tag { get; set; } = string.Empty;
        public string EquipmentName { get; set; } = string.Empty;

        // NOVOS CAMPOS DO CABEÇALHO
        public string? FactoryName { get; set; }     // Fábrica (ex.: Matozinhos)
        public string? RevisionInfo { get; set; }    // Data / Revisão

        public List<ParsedEquipamentoRow> Rows { get; set; } = new();
    }
}
