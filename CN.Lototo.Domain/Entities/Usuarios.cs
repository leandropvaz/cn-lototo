using CN.Lototo.Domain.Enums;

namespace CN.Lototo.Domain.Entities
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Login { get; set; } = default!;
        public string NomeCompleto { get; set; } = default!;
        public string SenhaHash { get; set; } = default!;
        public PerfilUsuario Perfil { get; set; }

        public bool Ativa { get; set; } = true;

        // Auditoria
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        // Vinculado à Planta (obrigatório para Usuário e Administrador)
        public int? PlantaId { get; set; }
        public Plantas? Planta { get; set; }

        public bool EhSuperGestor => Perfil == PerfilUsuario.SuperGestor;
    }
}
