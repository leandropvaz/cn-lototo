namespace CN.Lototo.Client.Dto
{
    public class EquipamentoDto
    {
        public int Id { get; set; }
        public string Tag { get; set; } = default!;
        public string Nome { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public string NomePlanta { get; set; } = default!;
        public string? UrlArquivoProcedimento { get; set; }
        public string? UrlImagem { get; set; }
    }
}
