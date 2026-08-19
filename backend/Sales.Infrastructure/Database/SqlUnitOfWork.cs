using Microsoft.Data.SqlClient;
using Sales.Application.Interfaces;

namespace Sales.Infrastructure.Database;

public sealed class SqlUnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DbTransactionContext _transactionContext;

    public IClientRepository Clients { get; }

    public IProductRepository Products { get; }

    public IOrderRepository Orders { get; }

    public SqlUnitOfWork(
        IDbConnectionFactory connectionFactory,
        IClientRepository clients,
        IProductRepository products,
        IOrderRepository orders,
        DbTransactionContext transactionContext)
    {
        _connectionFactory = connectionFactory;
        _transactionContext = transactionContext;

        Clients = clients;
        Products = products;
        Orders = orders;
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transactionContext.IsActive)
        {
            throw new InvalidOperationException(
                "Já existe uma transação ativa.");
        }

        var connection = _connectionFactory.CreateConnection();

        if (connection is SqlConnection sqlConnection)
        {
            await sqlConnection.OpenAsync(cancellationToken);
        }
        else
        {
            connection.Open();
        }

        var transaction = connection.BeginTransaction();

        _transactionContext.Start(
            connection,
            transaction);
    }

    public Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        _transactionContext.Transaction.Commit();

        return Task.CompletedTask;
    }

    public Task RollbackAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transactionContext.IsActive)
        {
            _transactionContext.Transaction.Rollback();
        }

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _transactionContext.Clear();

        return ValueTask.CompletedTask;
    }
}