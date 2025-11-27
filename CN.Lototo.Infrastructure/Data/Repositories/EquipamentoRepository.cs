using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CN.Lototo.Infrastructure.Data.Repositories
{
    public class EquipamentoRepository :  IEquipamentoRepository
    {
        private readonly LototoContext _context;

        public EquipamentoRepository(LototoContext context)
        {
            _context = context;
        }

        public Task<List<Equipamento>> GetByPlantAsync(int plantId, CancellationToken ct = default)
        {
            return _context.Equipamentos
                .Where(x => x.PlantaId == plantId && !x.IsDeleted)
                .OrderBy(x => x.Tag)
                .ThenBy(x => x.LineNumber)
                .ToListAsync(ct);
        }

        public Task<Equipamento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _context.Equipamentos.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddRangeAsync(IEnumerable<Equipamento> records, CancellationToken ct = default)
        {
            await _context.Equipamentos.AddRangeAsync(records, ct);
        }

        public async Task AddAsync(Equipamento record, CancellationToken ct = default)
        {
            await _context.Equipamentos.AddAsync(record, ct);
        }

        public Task UpdateAsync(Equipamento record, CancellationToken ct = default)
        {
            _context.Equipamentos.Update(record);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}