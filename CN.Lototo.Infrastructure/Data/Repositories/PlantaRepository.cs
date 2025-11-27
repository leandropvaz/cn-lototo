using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CN.Lototo.Infrastructure.Data.Repositories
{
    public class PlantaRepository : BaseRepository<Plantas>, IPlantaRepository
    {
        private readonly LototoContext _context;

        public PlantaRepository(LototoContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Plantas>> ListarAtivasAsync()
        {
            return await _context.Plantas
                .Where(p => p.Ativa)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Plantas?> ObterPorCodigoAsync(string codigo)
        {
            return await _context.Plantas
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Ativa);
        }

        public override void Delete(Plantas entity)
        {
            entity.Ativa = false;
            entity.DataAtualizacao = DateTime.UtcNow;
            _context.Plantas.Update(entity);
        }
    }
}
