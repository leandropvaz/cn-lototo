namespace CN.Lototo.Web.Dto
{
    public class LoginDto
    {
        public string Login { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public int? PlantaId { get; set; }
    }
}
