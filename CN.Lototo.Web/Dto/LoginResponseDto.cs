namespace CN.Lototo.Web.Dto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = default!;
        public string NomeUsuario { get; set; } = default!;
        public string Perfil { get; set; } = default!;
        public int? PlantaId { get; set; }
        public string? NomePlanta { get; set; }
    }
}
