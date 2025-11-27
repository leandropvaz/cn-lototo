using CN.Lototo.Domain.Entities;

namespace CN.Lototo.Domain.Interfaces
{
    public interface IEquipamentoRepository
    {
        Task<List<Equipamento>> GetByPlantAsync(int plantId, CancellationToken ct = default);
        Task<Equipamento?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task AddRangeAsync(IEnumerable<Equipamento> records, CancellationToken ct = default);
        Task AddAsync(Equipamento record, CancellationToken ct = default);
        Task UpdateAsync(Equipamento record, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
