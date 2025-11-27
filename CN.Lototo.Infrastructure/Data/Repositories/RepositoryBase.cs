using CN.Lototo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CN.Lototo.Infrastructure.Data.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly LototoContext Context;

        public BaseRepository(LototoContext context)
        {
            Context = context;
        }

        public async Task<T?> GetByIdAsync(int id)
            => await Context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAsync()
            => await Context.Set<T>().ToListAsync();

        public async Task AddAsync(T entidade)
        {
            await Context.Set<T>().AddAsync(entidade);
        }

        public void Update(T entidade)
        {
            Context.Set<T>().Update(entidade);
        }

        public virtual void Delete(T entidade)
        {
            Context.Set<T>().Remove(entidade);
        }
    }
}
