using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN.Lototo.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuarios>, IUsuarioRepository
    {
        public UsuarioRepository(LototoContext contexto) : base(contexto) { }

        public Task<Usuarios?> ObterPorLoginAsync(string login)
            => Context.Usuarios
                .Include(u => u.Planta)
                .FirstOrDefaultAsync(u => u.Login == login);

        public async Task<IReadOnlyList<Usuarios>> ListarPorPlantaAsync(int plantaId)
            => await Context.Usuarios
                .Where(u => u.PlantaId == plantaId)
                .OrderBy(u => u.NomeCompleto)
                .ToListAsync();
    }
}
