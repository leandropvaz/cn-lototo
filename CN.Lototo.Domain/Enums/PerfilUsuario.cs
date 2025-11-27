namespace CN.Lototo.Domain.Enums
{
    public enum PerfilUsuario
    {
        Usuario = 1,        // vê e pesquisa equipamentos da própria planta
        Administrador = 2,  // gerencia usuários e equipamento da planta
        SuperGestor = 3     // vê todas as plantas e cadastra administradores
    }
}
