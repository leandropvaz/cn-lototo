using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Lototo.Application.Dto
{
    public class PlantasDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Localizacao { get; set; } = string.Empty;
        public bool Ativa { get; set; }
    }
}
