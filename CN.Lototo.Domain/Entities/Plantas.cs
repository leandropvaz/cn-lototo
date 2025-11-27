namespace CN.Lototo.Domain.Entities
{
    public class Plantas
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;

        public string Localizacao { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;

        // Auditoria
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        public ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
        public ICollection<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
    }
}


