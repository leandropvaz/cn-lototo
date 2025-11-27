namespace CN.Lototo.Client.Dto
{
    public class LoginDto
    {
        public string Login { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public int? PlantaId { get; set; }
    }
}
