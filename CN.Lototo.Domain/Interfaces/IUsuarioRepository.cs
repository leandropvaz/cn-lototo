using CN.Lototo.Domain.Entities;

namespace CN.Lototo.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuarios>
    {
        Task<Usuarios?> ObterPorLoginAsync(string login);
        Task<IReadOnlyList<Usuarios>> ListarPorPlantaAsync(int plantaId);
    }
}
