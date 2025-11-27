namespace CN.Lototo.Application.Dto
{
    public class UsuariosDto
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string NomeCompleto { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty; // senha em texto (somente entrada)

        public int Perfil { get; set; } // enum int

        public bool Ativa { get; set; }

        public int? PlantaId { get; set; }
    }
}
