namespace Sales.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IClientRepository Clients { get; }

    IProductRepository Products { get; }

    IOrderRepository Orders { get; }

    Task BeginTransactionAsync(
        CancellationToken cancellationToken = default);

    Task CommitAsync(
        CancellationToken cancellationToken = default);

    Task RollbackAsync(
        CancellationToken cancellationToken = default);
}