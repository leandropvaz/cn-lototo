namespace CN.Lototo.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
