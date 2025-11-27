namespace CN.Lototo.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAsync();
        Task AddAsync(T entidade);
        void Update(T entidade);
        void Delete(T entidade);
    }
}
