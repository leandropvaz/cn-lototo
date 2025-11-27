using CN.Lototo.Domain.Entities;

namespace CN.Lototo.Domain.Interfaces
{
    public interface IPlantaRepository : IRepository<Plantas>
    {
        Task<IReadOnlyList<Plantas>> ListarAtivasAsync();
        Task<Plantas?> ObterPorCodigoAsync(string codigo);
    }
}
