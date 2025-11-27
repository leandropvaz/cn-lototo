using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Lototo.Application.Dto
{
    public class EquipamentoImportResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int ImportedCount { get; set; }
    }
}
